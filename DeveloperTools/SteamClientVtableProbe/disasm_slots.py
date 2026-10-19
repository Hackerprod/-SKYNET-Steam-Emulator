import json,sys,pefile
from capstone import *
p=pefile.PE(sys.argv[1],fast_load=True); base=p.OPTIONAL_HEADER.ImageBase
data=open(sys.argv[1],'rb').read()
def off(r):
    for s in p.sections:
        if s.VirtualAddress<=r<s.VirtualAddress+s.SizeOfRawData: return s.PointerToRawData+r-s.VirtualAddress
vt=json.load(open("steamcmd_bins/vt/out.json"))
md=Cs(CS_ARCH_X86,CS_MODE_64)
cls=sys.argv[2]; slots=[int(x) for x in sys.argv[3:]]
ents=vt[cls][0][1]
for s in slots:
    r=int(ents[s],16); o=off(r)
    print(f"--- {cls} slot {s} rva={r:x}")
    n=0
    for i in md.disasm(data[o:o+120],base+r):
        print("  %-8s %s"%(i.mnemonic,i.op_str)); n+=1
        if i.mnemonic in("ret","jmp") or n>14: break
