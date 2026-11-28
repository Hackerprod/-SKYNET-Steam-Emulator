using System.Text;
using System.Text.RegularExpressions;

namespace SKYNET.Iface;

/// <summary>One slot of a committed header-spec file: idx|Name|paramCount|returnType|"type name, type name".</summary>
internal sealed class SpecRow
{
    public int Index;
    public string Name = "";
    public string Ret = "";
    public string[] ParamTypes = new string[0];
    public string[] ParamNames = new string[0];
}

/// <summary>One method of one interface, with its signature resolved against the type tables. Skip != null = not patchable.</summary>
internal sealed class Plan
{
    public string Iface = "";
    public SpecRow Row = null!;
    public K Ret;
    public K[] P = new K[0];
    public string? Skip;

    public bool Hidden => Row.Ret == "CSteamID";                       // CSteamID returns come back through a hidden pointer after `this`
    public int Arity => 1 + (Hidden ? 1 : 0) + Row.ParamTypes.Length;   // machine arguments seen by the interposer
}

internal static class Spec
{
    // Methods traced when no --interfaces is given (the first interface verified live).
    public const string DefaultIface = "SteamFriends017";
    public static readonly string[] DefaultMethods =
    {
        "GetPersonaState", "GetFriendCount", "GetFriendByIndex", "GetFriendRelationship", "GetFriendPersonaState",
        "GetFriendPersonaName", "GetFriendGamePlayed", "GetFriendRichPresence", "HasFriend",
        "ActivateGameOverlayToUser", "RequestUserInformation",
    };

    public static List<SpecRow> Load(string path)
    {
        var rows = new List<SpecRow>();
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;
            var f = line.Split('|');
            if (f.Length < 4 || !int.TryParse(f[0], out var idx)) continue;
            var row = new SpecRow { Index = idx, Name = f[1], Ret = f[3].Trim() };
            var types = new List<string>(); var names = new List<string>();
            var ps = f.Length > 4 ? f[4].Trim() : "";
            if (ps.Length > 0)
                foreach (var p in ps.Split(','))
                {
                    var m = Regex.Match(p.Trim(), @"^(.*?)([A-Za-z_]\w*)$");
                    types.Add(m.Groups[1].Value.Trim()); names.Add(m.Groups[2].Value);
                }
            row.ParamTypes = types.ToArray(); row.ParamNames = names.ToArray();
            rows.Add(row);
        }
        return rows;
    }

    /// <summary>
    /// "SteamFriends017" (a spec file name), "ISteamFriends:017" or "ISteamFriends" (only when a single version exists)
    /// to the spec file name. An omitted version is refused when several exist: guessing would silently record nothing.
    /// </summary>
    public static string? Resolve(string specDir, string token, out string error)
    {
        error = "";
        token = token.Trim();
        var files = Directory.GetFiles(specDir, "*.txt").Select(Path.GetFileNameWithoutExtension).Select(s => s!).ToList();
        var exact = files.FirstOrDefault(f => string.Equals(f, token, StringComparison.OrdinalIgnoreCase));
        if (exact != null) return exact;
        var parts = token.Split(':');
        var name = Regex.Replace(parts[0], "^I(?=Steam)", "", RegexOptions.IgnoreCase);
        var rx = new Regex("^" + Regex.Escape(name) + @"(?:_INTERFACE_VERSION_?|_INTERFACE_V)?(\d+)$", RegexOptions.IgnoreCase);
        var cands = files.Where(f => rx.IsMatch(f)).OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
        if (parts.Length > 1 && int.TryParse(parts[1], out var ver))
        {
            var hit = cands.FirstOrDefault(f => int.Parse(rx.Match(f).Groups[1].Value) == ver);
            if (hit != null) return hit;
        }
        else if (cands.Count == 1) return cands[0];
        error = "interface '" + token + "': " + (cands.Count == 0 ? "no spec file matches" : "give a version, e.g. " + parts[0] + ":" + rx.Match(cands.Last()).Groups[1].Value + "; available: " + string.Join(", ", cands));
        return null;
    }

    public static Plan Build(string iface, SpecRow row)
    {
        var p = new Plan { Iface = iface, Row = row, P = new K[row.ParamTypes.Length] };
        var r = Types.Resolve(row.Ret, true, out var why);
        if (r == null) p.Skip = "return type: " + why; else p.Ret = r.Value;
        for (var i = 0; i < row.ParamTypes.Length && p.Skip == null; i++)
        {
            var k = Types.Resolve(row.ParamTypes[i], false, out why);
            if (k == null) p.Skip = "parameter " + row.ParamNames[i] + ": " + why; else p.P[i] = k.Value;
        }
        if (p.Skip == null && p.Arity > 8) p.Skip = "more than 8 machine arguments";
        return p;
    }

    /// <summary>Plans for the requested names, or for every method when names is empty; unsupported ones carry their Skip reason.</summary>
    public static List<Plan> Select(string iface, List<SpecRow> rows, string[] names)
    {
        var all = names.Length == 0;
        var plans = new List<Plan>();
        foreach (var row in rows)
            if (all || names.Contains(row.Name)) plans.Add(Build(iface, row));
        return plans;
    }
}

/// <summary>Turns register values into the strings of a record, driven by the Plan (no per-interface code).</summary>
internal static class Decoder
{
    public static string[] Args(Plan p, long[] regs, out string? flagNames)
    {
        flagNames = null;
        var p0 = p.Hidden ? 2 : 1;
        var list = new List<string>();
        for (var j = 0; j < p.P.Length; j++)
        {
            var raw = regs[p0 + j];
            var name = p.Row.ParamNames[j];
            if (p.P[j] == K.I32 && Types.FlagParams.TryGetValue(name, out var table))
            {
                // Hex like the emulator's GetFriendCount line; the diff parses numbers, so the format never matters there.
                list.Add("0x" + ((int)raw).ToString("X"));
                flagNames = Types.FlagNames(table, (int)raw);
            }
            else if (p.P[j] == K.Ptr && Types.OutStructs.ContainsKey(Types.Norm(p.Row.ParamTypes[j]))) continue; // reported in "o"
            else
            {
                var v = Types.Value(p.P[j], raw);
                list.Add(p.P[j] == K.Enum ? Types.EnumName(Types.Norm(p.Row.ParamTypes[j]), v) : v);
            }
        }
        return list.ToArray();
    }

    public static void Result(Plan p, long[] regs, long rax, Rec rec)
    {
        switch (p.Ret)
        {
            case K.Void: rec.R = ""; break;
            case K.SteamId:
                var id = Mem.ReadU64((IntPtr)regs[1]);
                rec.R = id.ToString(); rec.Rn = Types.SteamIdName(id);
                break;
            case K.Enum:
                rec.R = Types.Value(K.Enum, rax);
                var n = Types.EnumName(Types.Norm(p.Row.Ret), rec.R);
                if (n != rec.R) rec.Rn = n;
                break;
            default: rec.R = Types.Value(p.Ret, rax); break;
        }

        // Out structs are only valid after a successful call.
        if (p.Ret == K.Bool && rec.R != "true") return;
        var p0 = p.Hidden ? 2 : 1;
        for (var j = 0; j < p.P.Length; j++)
        {
            if (!Types.OutStructs.TryGetValue(Types.Norm(p.Row.ParamTypes[j]), out var os)) continue;
            var buf = Mem.Read((IntPtr)regs[p0 + j], os.Key);
            if (buf != null) rec.O = os.Value(buf);
        }
    }
}

/// <summary>One recorded call (decoded). Built in the detour, serialized by the writer thread.</summary>
internal sealed class Rec
{
    public long Seq, Q, QLast;
    public int Tid, Rep = 1;
    public string I = "", M = "", R = "";
    public string[] A = new string[0];
    public string? Rn, Af, O, C;
    public bool Self;

    public bool Same(Rec o) => I == o.I && M == o.M && R == o.R && Rn == o.Rn && O == o.O && Self == o.Self && A.SequenceEqual(o.A);

    public string ToJson()
    {
        var sb = new StringBuilder(192);
        sb.Append("{\"k\":\"call\",\"seq\":").Append(Seq).Append(",\"q\":").Append(Q).Append(",\"tid\":").Append(Tid);
        sb.Append(",\"i\":"); Json.Str(sb, I);
        sb.Append(",\"m\":"); Json.Str(sb, M);
        sb.Append(",\"a\":[");
        for (var i = 0; i < A.Length; i++) { if (i > 0) sb.Append(','); Json.Str(sb, A[i]); }
        sb.Append("],\"r\":"); Json.Str(sb, R);
        if (Rn != null) { sb.Append(",\"rn\":"); Json.Str(sb, Rn); }
        if (Af != null) { sb.Append(",\"af\":"); Json.Str(sb, Af); }
        if (O != null) sb.Append(",\"o\":").Append(O);
        if (C != null) { sb.Append(",\"c\":"); Json.Str(sb, C); }
        if (Self) sb.Append(",\"self\":true");
        if (Rep > 1) sb.Append(",\"rep\":").Append(Rep).Append(",\"qlast\":").Append(QLast);
        return sb.Append('}').ToString();
    }
}

internal static class Json
{
    public static void Str(StringBuilder sb, string? s)
    {
        if (s == null) { sb.Append("null"); return; }
        sb.Append('"');
        foreach (var ch in s)
        {
            switch (ch)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (ch < 0x20) sb.Append("\\u").Append(((int)ch).ToString("x4"));
                    else sb.Append(ch);
                    break;
            }
        }
        sb.Append('"');
    }

    public static string Str(string? s) { var sb = new StringBuilder(); Str(sb, s); return sb.ToString(); }

    public static string Arr(IEnumerable<string> items) => "[" + string.Join(",", items.Select(Str)) + "]";
}
