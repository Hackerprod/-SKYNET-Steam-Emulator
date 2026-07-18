"use strict";
(() => {
  var __create = Object.create;
  var __defProp = Object.defineProperty;
  var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __getProtoOf = Object.getPrototypeOf;
  var __hasOwnProp = Object.prototype.hasOwnProperty;
  var __require = /* @__PURE__ */ ((x) => typeof require !== "undefined" ? require : typeof Proxy !== "undefined" ? new Proxy(x, {
    get: (a, b) => (typeof require !== "undefined" ? require : a)[b]
  }) : x)(function(x) {
    if (typeof require !== "undefined") return require.apply(this, arguments);
    throw Error('Dynamic require of "' + x + '" is not supported');
  });
  var __commonJS = (cb, mod) => function __require2() {
    return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
  };
  var __copyProps = (to, from, except, desc) => {
    if (from && typeof from === "object" || typeof from === "function") {
      for (let key of __getOwnPropNames(from))
        if (!__hasOwnProp.call(to, key) && key !== except)
          __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
    }
    return to;
  };
  var __toESM = (mod, isNodeMode, target) => (target = mod != null ? __create(__getProtoOf(mod)) : {}, __copyProps(
    // If the importer is in node compatibility mode or this is not an ESM
    // file that has been converted to a CommonJS file using a Babel-
    // compatible transform (i.e. "__esModule" has not been set), then set
    // "default" to the CommonJS "module.exports" for node compatibility.
    isNodeMode || !mod || !mod.__esModule ? __defProp(target, "default", { value: mod, enumerable: true }) : target,
    mod
  ));

  // node_modules/@protobufjs/aspromise/index.js
  var require_aspromise = __commonJS({
    "node_modules/@protobufjs/aspromise/index.js"(exports, module) {
      "use strict";
      module.exports = asPromise;
      function asPromise(fn, ctx) {
        var params = new Array(arguments.length - 1), offset = 0, index = 2, pending = true;
        while (index < arguments.length)
          params[offset++] = arguments[index++];
        return new Promise(function executor(resolve, reject) {
          params[offset] = function callback(err) {
            if (pending) {
              pending = false;
              if (err)
                reject(err);
              else {
                var params2 = new Array(arguments.length - 1), offset2 = 0;
                while (offset2 < params2.length)
                  params2[offset2++] = arguments[offset2];
                resolve.apply(null, params2);
              }
            }
          };
          try {
            fn.apply(ctx || null, params);
          } catch (err) {
            if (pending) {
              pending = false;
              reject(err);
            }
          }
        });
      }
    }
  });

  // node_modules/@protobufjs/base64/index.js
  var require_base64 = __commonJS({
    "node_modules/@protobufjs/base64/index.js"(exports) {
      "use strict";
      var base64 = exports;
      base64.length = function length(string) {
        var p = string.length;
        if (!p)
          return 0;
        var n = 0;
        while (--p % 4 > 1 && string.charAt(p) === "=")
          ++n;
        return Math.ceil(string.length * 3) / 4 - n;
      };
      var b64 = new Array(64);
      var s64 = new Array(123);
      for (i = 0; i < 64; )
        s64[b64[i] = i < 26 ? i + 65 : i < 52 ? i + 71 : i < 62 ? i - 4 : i - 59 | 43] = i++;
      var i;
      base64.encode = function encode(buffer, start, end) {
        var parts = null, chunk = [];
        var i2 = 0, j = 0, t;
        while (start < end) {
          var b = buffer[start++];
          switch (j) {
            case 0:
              chunk[i2++] = b64[b >> 2];
              t = (b & 3) << 4;
              j = 1;
              break;
            case 1:
              chunk[i2++] = b64[t | b >> 4];
              t = (b & 15) << 2;
              j = 2;
              break;
            case 2:
              chunk[i2++] = b64[t | b >> 6];
              chunk[i2++] = b64[b & 63];
              j = 0;
              break;
          }
          if (i2 > 8191) {
            (parts || (parts = [])).push(String.fromCharCode.apply(String, chunk));
            i2 = 0;
          }
        }
        if (j) {
          chunk[i2++] = b64[t];
          chunk[i2++] = 61;
          if (j === 1)
            chunk[i2++] = 61;
        }
        if (parts) {
          if (i2)
            parts.push(String.fromCharCode.apply(String, chunk.slice(0, i2)));
          return parts.join("");
        }
        return String.fromCharCode.apply(String, chunk.slice(0, i2));
      };
      var invalidEncoding = "invalid encoding";
      base64.decode = function decode(string, buffer, offset) {
        var start = offset;
        var j = 0, t;
        for (var i2 = 0; i2 < string.length; ) {
          var c = string.charCodeAt(i2++);
          if (c === 61 && j > 1)
            break;
          if ((c = s64[c]) === void 0)
            throw Error(invalidEncoding);
          switch (j) {
            case 0:
              t = c;
              j = 1;
              break;
            case 1:
              buffer[offset++] = t << 2 | (c & 48) >> 4;
              t = c;
              j = 2;
              break;
            case 2:
              buffer[offset++] = (t & 15) << 4 | (c & 60) >> 2;
              t = c;
              j = 3;
              break;
            case 3:
              buffer[offset++] = (t & 3) << 6 | c;
              j = 0;
              break;
          }
        }
        if (j === 1)
          throw Error(invalidEncoding);
        return offset - start;
      };
      base64.test = function test(string) {
        return /^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$/.test(string);
      };
    }
  });

  // node_modules/@protobufjs/eventemitter/index.js
  var require_eventemitter = __commonJS({
    "node_modules/@protobufjs/eventemitter/index.js"(exports, module) {
      "use strict";
      module.exports = EventEmitter;
      function EventEmitter() {
        this._listeners = /* @__PURE__ */ Object.create(null);
      }
      EventEmitter.prototype.on = function on(evt, fn, ctx) {
        (this._listeners[evt] || (this._listeners[evt] = [])).push({
          fn,
          ctx: ctx || this
        });
        return this;
      };
      EventEmitter.prototype.off = function off(evt, fn) {
        if (evt === void 0)
          this._listeners = /* @__PURE__ */ Object.create(null);
        else {
          if (fn === void 0)
            this._listeners[evt] = [];
          else {
            var listeners = this._listeners[evt];
            if (!listeners)
              return this;
            for (var i = 0; i < listeners.length; )
              if (listeners[i].fn === fn)
                listeners.splice(i, 1);
              else
                ++i;
          }
        }
        return this;
      };
      EventEmitter.prototype.emit = function emit(evt) {
        var listeners = this._listeners[evt];
        if (listeners) {
          var args = [], i = 1;
          for (; i < arguments.length; )
            args.push(arguments[i++]);
          for (i = 0; i < listeners.length; )
            listeners[i].fn.apply(listeners[i++].ctx, args);
        }
        return this;
      };
    }
  });

  // node_modules/@protobufjs/float/index.js
  var require_float = __commonJS({
    "node_modules/@protobufjs/float/index.js"(exports, module) {
      "use strict";
      module.exports = factory(factory);
      function factory(exports2) {
        if (typeof Float32Array !== "undefined") (function() {
          var f32 = new Float32Array([-0]), f8b = new Uint8Array(f32.buffer), le = f8b[3] === 128;
          function writeFloat_f32_cpy(val, buf, pos) {
            f32[0] = val;
            buf[pos] = f8b[0];
            buf[pos + 1] = f8b[1];
            buf[pos + 2] = f8b[2];
            buf[pos + 3] = f8b[3];
          }
          function writeFloat_f32_rev(val, buf, pos) {
            f32[0] = val;
            buf[pos] = f8b[3];
            buf[pos + 1] = f8b[2];
            buf[pos + 2] = f8b[1];
            buf[pos + 3] = f8b[0];
          }
          exports2.writeFloatLE = le ? writeFloat_f32_cpy : writeFloat_f32_rev;
          exports2.writeFloatBE = le ? writeFloat_f32_rev : writeFloat_f32_cpy;
          function readFloat_f32_cpy(buf, pos) {
            f8b[0] = buf[pos];
            f8b[1] = buf[pos + 1];
            f8b[2] = buf[pos + 2];
            f8b[3] = buf[pos + 3];
            return f32[0];
          }
          function readFloat_f32_rev(buf, pos) {
            f8b[3] = buf[pos];
            f8b[2] = buf[pos + 1];
            f8b[1] = buf[pos + 2];
            f8b[0] = buf[pos + 3];
            return f32[0];
          }
          exports2.readFloatLE = le ? readFloat_f32_cpy : readFloat_f32_rev;
          exports2.readFloatBE = le ? readFloat_f32_rev : readFloat_f32_cpy;
        })();
        else (function() {
          function writeFloat_ieee754(writeUint, val, buf, pos) {
            var sign = val < 0 ? 1 : 0;
            if (sign)
              val = -val;
            if (val === 0)
              writeUint(1 / val > 0 ? (
                /* positive */
                0
              ) : (
                /* negative 0 */
                2147483648
              ), buf, pos);
            else if (isNaN(val))
              writeUint(2143289344, buf, pos);
            else if (val > 34028234663852886e22)
              writeUint((sign << 31 | 2139095040) >>> 0, buf, pos);
            else if (val < 11754943508222875e-54)
              writeUint((sign << 31 | Math.round(val / 1401298464324817e-60)) >>> 0, buf, pos);
            else {
              var exponent = Math.floor(Math.log(val) / Math.LN2), mantissa = Math.round(val * Math.pow(2, -exponent) * 8388608) & 8388607;
              writeUint((sign << 31 | exponent + 127 << 23 | mantissa) >>> 0, buf, pos);
            }
          }
          exports2.writeFloatLE = writeFloat_ieee754.bind(null, writeUintLE);
          exports2.writeFloatBE = writeFloat_ieee754.bind(null, writeUintBE);
          function readFloat_ieee754(readUint, buf, pos) {
            var uint = readUint(buf, pos), sign = (uint >> 31) * 2 + 1, exponent = uint >>> 23 & 255, mantissa = uint & 8388607;
            return exponent === 255 ? mantissa ? NaN : sign * Infinity : exponent === 0 ? sign * 1401298464324817e-60 * mantissa : sign * Math.pow(2, exponent - 150) * (mantissa + 8388608);
          }
          exports2.readFloatLE = readFloat_ieee754.bind(null, readUintLE);
          exports2.readFloatBE = readFloat_ieee754.bind(null, readUintBE);
        })();
        if (typeof Float64Array !== "undefined") (function() {
          var f64 = new Float64Array([-0]), f8b = new Uint8Array(f64.buffer), le = f8b[7] === 128;
          function writeDouble_f64_cpy(val, buf, pos) {
            f64[0] = val;
            buf[pos] = f8b[0];
            buf[pos + 1] = f8b[1];
            buf[pos + 2] = f8b[2];
            buf[pos + 3] = f8b[3];
            buf[pos + 4] = f8b[4];
            buf[pos + 5] = f8b[5];
            buf[pos + 6] = f8b[6];
            buf[pos + 7] = f8b[7];
          }
          function writeDouble_f64_rev(val, buf, pos) {
            f64[0] = val;
            buf[pos] = f8b[7];
            buf[pos + 1] = f8b[6];
            buf[pos + 2] = f8b[5];
            buf[pos + 3] = f8b[4];
            buf[pos + 4] = f8b[3];
            buf[pos + 5] = f8b[2];
            buf[pos + 6] = f8b[1];
            buf[pos + 7] = f8b[0];
          }
          exports2.writeDoubleLE = le ? writeDouble_f64_cpy : writeDouble_f64_rev;
          exports2.writeDoubleBE = le ? writeDouble_f64_rev : writeDouble_f64_cpy;
          function readDouble_f64_cpy(buf, pos) {
            f8b[0] = buf[pos];
            f8b[1] = buf[pos + 1];
            f8b[2] = buf[pos + 2];
            f8b[3] = buf[pos + 3];
            f8b[4] = buf[pos + 4];
            f8b[5] = buf[pos + 5];
            f8b[6] = buf[pos + 6];
            f8b[7] = buf[pos + 7];
            return f64[0];
          }
          function readDouble_f64_rev(buf, pos) {
            f8b[7] = buf[pos];
            f8b[6] = buf[pos + 1];
            f8b[5] = buf[pos + 2];
            f8b[4] = buf[pos + 3];
            f8b[3] = buf[pos + 4];
            f8b[2] = buf[pos + 5];
            f8b[1] = buf[pos + 6];
            f8b[0] = buf[pos + 7];
            return f64[0];
          }
          exports2.readDoubleLE = le ? readDouble_f64_cpy : readDouble_f64_rev;
          exports2.readDoubleBE = le ? readDouble_f64_rev : readDouble_f64_cpy;
        })();
        else (function() {
          function writeDouble_ieee754(writeUint, off0, off1, val, buf, pos) {
            var sign = val < 0 ? 1 : 0;
            if (sign)
              val = -val;
            if (val === 0) {
              writeUint(0, buf, pos + off0);
              writeUint(1 / val > 0 ? (
                /* positive */
                0
              ) : (
                /* negative 0 */
                2147483648
              ), buf, pos + off1);
            } else if (isNaN(val)) {
              writeUint(0, buf, pos + off0);
              writeUint(2146959360, buf, pos + off1);
            } else if (val > 17976931348623157e292) {
              writeUint(0, buf, pos + off0);
              writeUint((sign << 31 | 2146435072) >>> 0, buf, pos + off1);
            } else {
              var mantissa;
              if (val < 22250738585072014e-324) {
                mantissa = val / 5e-324;
                writeUint(mantissa >>> 0, buf, pos + off0);
                writeUint((sign << 31 | mantissa / 4294967296) >>> 0, buf, pos + off1);
              } else {
                var exponent = Math.floor(Math.log(val) / Math.LN2);
                if (exponent === 1024)
                  exponent = 1023;
                mantissa = val * Math.pow(2, -exponent);
                writeUint(mantissa * 4503599627370496 >>> 0, buf, pos + off0);
                writeUint((sign << 31 | exponent + 1023 << 20 | mantissa * 1048576 & 1048575) >>> 0, buf, pos + off1);
              }
            }
          }
          exports2.writeDoubleLE = writeDouble_ieee754.bind(null, writeUintLE, 0, 4);
          exports2.writeDoubleBE = writeDouble_ieee754.bind(null, writeUintBE, 4, 0);
          function readDouble_ieee754(readUint, off0, off1, buf, pos) {
            var lo = readUint(buf, pos + off0), hi = readUint(buf, pos + off1);
            var sign = (hi >> 31) * 2 + 1, exponent = hi >>> 20 & 2047, mantissa = 4294967296 * (hi & 1048575) + lo;
            return exponent === 2047 ? mantissa ? NaN : sign * Infinity : exponent === 0 ? sign * 5e-324 * mantissa : sign * Math.pow(2, exponent - 1075) * (mantissa + 4503599627370496);
          }
          exports2.readDoubleLE = readDouble_ieee754.bind(null, readUintLE, 0, 4);
          exports2.readDoubleBE = readDouble_ieee754.bind(null, readUintBE, 4, 0);
        })();
        return exports2;
      }
      function writeUintLE(val, buf, pos) {
        buf[pos] = val & 255;
        buf[pos + 1] = val >>> 8 & 255;
        buf[pos + 2] = val >>> 16 & 255;
        buf[pos + 3] = val >>> 24;
      }
      function writeUintBE(val, buf, pos) {
        buf[pos] = val >>> 24;
        buf[pos + 1] = val >>> 16 & 255;
        buf[pos + 2] = val >>> 8 & 255;
        buf[pos + 3] = val & 255;
      }
      function readUintLE(buf, pos) {
        return (buf[pos] | buf[pos + 1] << 8 | buf[pos + 2] << 16 | buf[pos + 3] << 24) >>> 0;
      }
      function readUintBE(buf, pos) {
        return (buf[pos] << 24 | buf[pos + 1] << 16 | buf[pos + 2] << 8 | buf[pos + 3]) >>> 0;
      }
    }
  });

  // node_modules/@protobufjs/inquire/index.js
  var require_inquire = __commonJS({
    "node_modules/@protobufjs/inquire/index.js"(exports, module) {
      "use strict";
      module.exports = inquire;
      function inquire(moduleName) {
        try {
          if (typeof __require !== "function") {
            return null;
          }
          var mod = __require(moduleName);
          if (mod && (mod.length || Object.keys(mod).length)) return mod;
          return null;
        } catch (err) {
          return null;
        }
      }
    }
  });

  // node_modules/@protobufjs/utf8/index.js
  var require_utf8 = __commonJS({
    "node_modules/@protobufjs/utf8/index.js"(exports) {
      "use strict";
      var utf8 = exports;
      var replacementCharCode = 65533;
      utf8.length = function utf8_length(string) {
        var len = 0, c = 0;
        for (var i = 0; i < string.length; ++i) {
          c = string.charCodeAt(i);
          if (c < 128)
            len += 1;
          else if (c < 2048)
            len += 2;
          else if ((c & 64512) === 55296 && (string.charCodeAt(i + 1) & 64512) === 56320) {
            ++i;
            len += 4;
          } else
            len += 3;
        }
        return len;
      };
      utf8.read = function utf8_read(buffer, start, end) {
        if (end - start < 1)
          return "";
        var parts = null, chunk = [], i = 0, t, t2, c2, c3;
        while (start < end) {
          t = buffer[start++];
          if (t <= 127) {
            chunk[i++] = t;
          } else if (t >= 192 && t < 224) {
            c2 = (t & 31) << 6 | buffer[start++] & 63;
            chunk[i++] = c2 >= 128 ? c2 : replacementCharCode;
          } else if (t >= 224 && t < 240) {
            c3 = (t & 15) << 12 | (buffer[start++] & 63) << 6 | buffer[start++] & 63;
            chunk[i++] = c3 >= 2048 ? c3 : replacementCharCode;
          } else if (t >= 240) {
            t2 = (t & 7) << 18 | (buffer[start++] & 63) << 12 | (buffer[start++] & 63) << 6 | buffer[start++] & 63;
            if (t2 < 65536 || t2 > 1114111)
              chunk[i++] = replacementCharCode;
            else {
              t2 -= 65536;
              chunk[i++] = 55296 + (t2 >> 10);
              chunk[i++] = 56320 + (t2 & 1023);
            }
          }
          if (i > 8191) {
            (parts || (parts = [])).push(String.fromCharCode.apply(String, chunk.slice(0, i)));
            i = 0;
          }
        }
        if (parts) {
          if (i)
            parts.push(String.fromCharCode.apply(String, chunk.slice(0, i)));
          return parts.join("");
        }
        return String.fromCharCode.apply(String, chunk.slice(0, i));
      };
      utf8.write = function utf8_write(string, buffer, offset) {
        var start = offset, c1, c2;
        for (var i = 0; i < string.length; ++i) {
          c1 = string.charCodeAt(i);
          if (c1 < 128) {
            buffer[offset++] = c1;
          } else if (c1 < 2048) {
            buffer[offset++] = c1 >> 6 | 192;
            buffer[offset++] = c1 & 63 | 128;
          } else if ((c1 & 64512) === 55296 && ((c2 = string.charCodeAt(i + 1)) & 64512) === 56320) {
            c1 = 65536 + ((c1 & 1023) << 10) + (c2 & 1023);
            ++i;
            buffer[offset++] = c1 >> 18 | 240;
            buffer[offset++] = c1 >> 12 & 63 | 128;
            buffer[offset++] = c1 >> 6 & 63 | 128;
            buffer[offset++] = c1 & 63 | 128;
          } else {
            buffer[offset++] = c1 >> 12 | 224;
            buffer[offset++] = c1 >> 6 & 63 | 128;
            buffer[offset++] = c1 & 63 | 128;
          }
        }
        return offset - start;
      };
    }
  });

  // node_modules/@protobufjs/pool/index.js
  var require_pool = __commonJS({
    "node_modules/@protobufjs/pool/index.js"(exports, module) {
      "use strict";
      module.exports = pool;
      function pool(alloc, slice, size) {
        var SIZE = size || 8192;
        var MAX = SIZE >>> 1;
        var slab = null;
        var offset = SIZE;
        return function pool_alloc(size2) {
          if (size2 < 1 || size2 > MAX)
            return alloc(size2);
          if (offset + size2 > SIZE) {
            slab = alloc(SIZE);
            offset = 0;
          }
          var buf = slice.call(slab, offset, offset += size2);
          if (offset & 7)
            offset = (offset | 7) + 1;
          return buf;
        };
      }
    }
  });

  // node_modules/protobufjs/src/util/longbits.js
  var require_longbits = __commonJS({
    "node_modules/protobufjs/src/util/longbits.js"(exports, module) {
      "use strict";
      module.exports = LongBits;
      var util2 = require_minimal();
      function LongBits(lo, hi) {
        this.lo = lo >>> 0;
        this.hi = hi >>> 0;
      }
      var zero = LongBits.zero = new LongBits(0, 0);
      zero.toNumber = function() {
        return 0;
      };
      zero.zzEncode = zero.zzDecode = function() {
        return this;
      };
      zero.length = function() {
        return 1;
      };
      var zeroHash = LongBits.zeroHash = "\0\0\0\0\0\0\0\0";
      LongBits.fromNumber = function fromNumber2(value) {
        if (value === 0)
          return zero;
        var sign = value < 0;
        if (sign)
          value = -value;
        var lo = value >>> 0, hi = (value - lo) / 4294967296 >>> 0;
        if (sign) {
          hi = ~hi >>> 0;
          lo = ~lo >>> 0;
          if (++lo > 4294967295) {
            lo = 0;
            if (++hi > 4294967295)
              hi = 0;
          }
        }
        return new LongBits(lo, hi);
      };
      LongBits.from = function from(value) {
        if (typeof value === "number")
          return LongBits.fromNumber(value);
        if (util2.isString(value)) {
          if (util2.Long)
            value = util2.Long.fromString(value);
          else
            return LongBits.fromNumber(parseInt(value, 10));
        }
        return value.low || value.high ? new LongBits(value.low >>> 0, value.high >>> 0) : zero;
      };
      LongBits.prototype.toNumber = function toNumber2(unsigned) {
        if (!unsigned && this.hi >>> 31) {
          var lo = ~this.lo + 1 >>> 0, hi = ~this.hi >>> 0;
          if (!lo)
            hi = hi + 1 >>> 0;
          return -(lo + hi * 4294967296);
        }
        return this.lo + this.hi * 4294967296;
      };
      LongBits.prototype.toLong = function toLong(unsigned) {
        return util2.Long ? new util2.Long(this.lo | 0, this.hi | 0, Boolean(unsigned)) : { low: this.lo | 0, high: this.hi | 0, unsigned: Boolean(unsigned) };
      };
      var charCodeAt = String.prototype.charCodeAt;
      LongBits.fromHash = function fromHash(hash) {
        if (hash === zeroHash)
          return zero;
        return new LongBits(
          (charCodeAt.call(hash, 0) | charCodeAt.call(hash, 1) << 8 | charCodeAt.call(hash, 2) << 16 | charCodeAt.call(hash, 3) << 24) >>> 0,
          (charCodeAt.call(hash, 4) | charCodeAt.call(hash, 5) << 8 | charCodeAt.call(hash, 6) << 16 | charCodeAt.call(hash, 7) << 24) >>> 0
        );
      };
      LongBits.prototype.toHash = function toHash() {
        return String.fromCharCode(
          this.lo & 255,
          this.lo >>> 8 & 255,
          this.lo >>> 16 & 255,
          this.lo >>> 24,
          this.hi & 255,
          this.hi >>> 8 & 255,
          this.hi >>> 16 & 255,
          this.hi >>> 24
        );
      };
      LongBits.prototype.zzEncode = function zzEncode() {
        var mask = this.hi >> 31;
        this.hi = ((this.hi << 1 | this.lo >>> 31) ^ mask) >>> 0;
        this.lo = (this.lo << 1 ^ mask) >>> 0;
        return this;
      };
      LongBits.prototype.zzDecode = function zzDecode() {
        var mask = -(this.lo & 1);
        this.lo = ((this.lo >>> 1 | this.hi << 31) ^ mask) >>> 0;
        this.hi = (this.hi >>> 1 ^ mask) >>> 0;
        return this;
      };
      LongBits.prototype.length = function length() {
        var part0 = this.lo, part1 = (this.lo >>> 28 | this.hi << 4) >>> 0, part2 = this.hi >>> 24;
        return part2 === 0 ? part1 === 0 ? part0 < 16384 ? part0 < 128 ? 1 : 2 : part0 < 2097152 ? 3 : 4 : part1 < 16384 ? part1 < 128 ? 5 : 6 : part1 < 2097152 ? 7 : 8 : part2 < 128 ? 9 : 10;
      };
    }
  });

  // node_modules/protobufjs/src/util/minimal.js
  var require_minimal = __commonJS({
    "node_modules/protobufjs/src/util/minimal.js"(exports) {
      "use strict";
      var util2 = exports;
      util2.asPromise = require_aspromise();
      util2.base64 = require_base64();
      util2.EventEmitter = require_eventemitter();
      util2.float = require_float();
      util2.inquire = require_inquire();
      util2.utf8 = require_utf8();
      util2.pool = require_pool();
      util2.LongBits = require_longbits();
      util2.isNode = Boolean(typeof global !== "undefined" && global && global.process && global.process.versions && global.process.versions.node);
      util2.global = util2.isNode && global || typeof window !== "undefined" && window || typeof self !== "undefined" && self || exports;
      util2.emptyArray = Object.freeze ? Object.freeze([]) : (
        /* istanbul ignore next */
        []
      );
      util2.emptyObject = Object.freeze ? Object.freeze({}) : (
        /* istanbul ignore next */
        {}
      );
      util2.isInteger = Number.isInteger || /* istanbul ignore next */
      function isInteger(value) {
        return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
      };
      util2.isString = function isString(value) {
        return typeof value === "string" || value instanceof String;
      };
      util2.isObject = function isObject(value) {
        return value && typeof value === "object";
      };
      util2.isset = /**
       * Checks if a property on a message is considered to be present.
       * @param {Object} obj Plain object or message instance
       * @param {string} prop Property name
       * @returns {boolean} `true` if considered to be present, otherwise `false`
       */
      util2.isSet = function isSet(obj, prop) {
        var value = obj[prop];
        if (value != null && obj.hasOwnProperty(prop))
          return typeof value !== "object" || (Array.isArray(value) ? value.length : Object.keys(value).length) > 0;
        return false;
      };
      util2.Buffer = function() {
        try {
          var Buffer2 = util2.inquire("buffer").Buffer;
          return Buffer2.prototype.utf8Write ? Buffer2 : (
            /* istanbul ignore next */
            null
          );
        } catch (e) {
          return null;
        }
      }();
      util2._Buffer_from = null;
      util2._Buffer_allocUnsafe = null;
      util2.newBuffer = function newBuffer(sizeOrArray) {
        return typeof sizeOrArray === "number" ? util2.Buffer ? util2._Buffer_allocUnsafe(sizeOrArray) : new util2.Array(sizeOrArray) : util2.Buffer ? util2._Buffer_from(sizeOrArray) : typeof Uint8Array === "undefined" ? sizeOrArray : new Uint8Array(sizeOrArray);
      };
      util2.Array = typeof Uint8Array !== "undefined" ? Uint8Array : Array;
      util2.Long = /* istanbul ignore next */
      util2.global.dcodeIO && /* istanbul ignore next */
      util2.global.dcodeIO.Long || /* istanbul ignore next */
      util2.global.Long || util2.inquire("long");
      util2.key2Re = /^true|false|0|1$/;
      util2.key32Re = /^-?(?:0|[1-9][0-9]*)$/;
      util2.key64Re = /^(?:[\\x00-\\xff]{8}|-?(?:0|[1-9][0-9]*))$/;
      util2.longToHash = function longToHash(value) {
        return value ? util2.LongBits.from(value).toHash() : util2.LongBits.zeroHash;
      };
      util2.longFromHash = function longFromHash(hash, unsigned) {
        var bits = util2.LongBits.fromHash(hash);
        if (util2.Long)
          return util2.Long.fromBits(bits.lo, bits.hi, unsigned);
        return bits.toNumber(Boolean(unsigned));
      };
      function merge(dst, src, ifNotSet) {
        for (var keys = Object.keys(src), i = 0; i < keys.length; ++i)
          if (dst[keys[i]] === void 0 || !ifNotSet)
            dst[keys[i]] = src[keys[i]];
        return dst;
      }
      util2.merge = merge;
      util2.lcFirst = function lcFirst(str) {
        return str.charAt(0).toLowerCase() + str.substring(1);
      };
      function newError(name) {
        function CustomError(message, properties) {
          if (!(this instanceof CustomError))
            return new CustomError(message, properties);
          Object.defineProperty(this, "message", { get: function() {
            return message;
          } });
          if (Error.captureStackTrace)
            Error.captureStackTrace(this, CustomError);
          else
            Object.defineProperty(this, "stack", { value: new Error().stack || "" });
          if (properties)
            merge(this, properties);
        }
        CustomError.prototype = Object.create(Error.prototype, {
          constructor: {
            value: CustomError,
            writable: true,
            enumerable: false,
            configurable: true
          },
          name: {
            get: function get() {
              return name;
            },
            set: void 0,
            enumerable: false,
            // configurable: false would accurately preserve the behavior of
            // the original, but I'm guessing that was not intentional.
            // For an actual error subclass, this property would
            // be configurable.
            configurable: true
          },
          toString: {
            value: function value() {
              return this.name + ": " + this.message;
            },
            writable: true,
            enumerable: false,
            configurable: true
          }
        });
        return CustomError;
      }
      util2.newError = newError;
      util2.ProtocolError = newError("ProtocolError");
      util2.oneOfGetter = function getOneOf(fieldNames) {
        var fieldMap = {};
        for (var i = 0; i < fieldNames.length; ++i)
          fieldMap[fieldNames[i]] = 1;
        return function() {
          for (var keys = Object.keys(this), i2 = keys.length - 1; i2 > -1; --i2)
            if (fieldMap[keys[i2]] === 1 && this[keys[i2]] !== void 0 && this[keys[i2]] !== null)
              return keys[i2];
        };
      };
      util2.oneOfSetter = function setOneOf(fieldNames) {
        return function(name) {
          for (var i = 0; i < fieldNames.length; ++i)
            if (fieldNames[i] !== name)
              delete this[fieldNames[i]];
        };
      };
      util2.toJSONOptions = {
        longs: String,
        enums: String,
        bytes: String,
        json: true
      };
      util2._configure = function() {
        var Buffer2 = util2.Buffer;
        if (!Buffer2) {
          util2._Buffer_from = util2._Buffer_allocUnsafe = null;
          return;
        }
        util2._Buffer_from = Buffer2.from !== Uint8Array.from && Buffer2.from || /* istanbul ignore next */
        function Buffer_from(value, encoding) {
          return new Buffer2(value, encoding);
        };
        util2._Buffer_allocUnsafe = Buffer2.allocUnsafe || /* istanbul ignore next */
        function Buffer_allocUnsafe(size) {
          return new Buffer2(size);
        };
      };
    }
  });

  // node_modules/protobufjs/src/writer.js
  var require_writer = __commonJS({
    "node_modules/protobufjs/src/writer.js"(exports, module) {
      "use strict";
      module.exports = Writer;
      var util2 = require_minimal();
      var BufferWriter;
      var LongBits = util2.LongBits;
      var base64 = util2.base64;
      var utf8 = util2.utf8;
      function Op(fn, len, val) {
        this.fn = fn;
        this.len = len;
        this.next = void 0;
        this.val = val;
      }
      function noop() {
      }
      function State(writer) {
        this.head = writer.head;
        this.tail = writer.tail;
        this.len = writer.len;
        this.next = writer.states;
      }
      function Writer() {
        this.len = 0;
        this.head = new Op(noop, 0, 0);
        this.tail = this.head;
        this.states = null;
      }
      var create = function create2() {
        return util2.Buffer ? function create_buffer_setup() {
          return (Writer.create = function create_buffer() {
            return new BufferWriter();
          })();
        } : function create_array() {
          return new Writer();
        };
      };
      Writer.create = create();
      Writer.alloc = function alloc(size) {
        return new util2.Array(size);
      };
      if (util2.Array !== Array)
        Writer.alloc = util2.pool(Writer.alloc, util2.Array.prototype.subarray);
      Writer.prototype._push = function push(fn, len, val) {
        this.tail = this.tail.next = new Op(fn, len, val);
        this.len += len;
        return this;
      };
      function writeByte(val, buf, pos) {
        buf[pos] = val & 255;
      }
      function writeVarint32(val, buf, pos) {
        while (val > 127) {
          buf[pos++] = val & 127 | 128;
          val >>>= 7;
        }
        buf[pos] = val;
      }
      function VarintOp(len, val) {
        this.len = len;
        this.next = void 0;
        this.val = val;
      }
      VarintOp.prototype = Object.create(Op.prototype);
      VarintOp.prototype.fn = writeVarint32;
      Writer.prototype.uint32 = function write_uint32(value) {
        this.len += (this.tail = this.tail.next = new VarintOp(
          (value = value >>> 0) < 128 ? 1 : value < 16384 ? 2 : value < 2097152 ? 3 : value < 268435456 ? 4 : 5,
          value
        )).len;
        return this;
      };
      Writer.prototype.int32 = function write_int32(value) {
        return value < 0 ? this._push(writeVarint64, 10, LongBits.fromNumber(value)) : this.uint32(value);
      };
      Writer.prototype.sint32 = function write_sint32(value) {
        return this.uint32((value << 1 ^ value >> 31) >>> 0);
      };
      function writeVarint64(val, buf, pos) {
        while (val.hi) {
          buf[pos++] = val.lo & 127 | 128;
          val.lo = (val.lo >>> 7 | val.hi << 25) >>> 0;
          val.hi >>>= 7;
        }
        while (val.lo > 127) {
          buf[pos++] = val.lo & 127 | 128;
          val.lo = val.lo >>> 7;
        }
        buf[pos++] = val.lo;
      }
      Writer.prototype.uint64 = function write_uint64(value) {
        var bits = LongBits.from(value);
        return this._push(writeVarint64, bits.length(), bits);
      };
      Writer.prototype.int64 = Writer.prototype.uint64;
      Writer.prototype.sint64 = function write_sint64(value) {
        var bits = LongBits.from(value).zzEncode();
        return this._push(writeVarint64, bits.length(), bits);
      };
      Writer.prototype.bool = function write_bool(value) {
        return this._push(writeByte, 1, value ? 1 : 0);
      };
      function writeFixed32(val, buf, pos) {
        buf[pos] = val & 255;
        buf[pos + 1] = val >>> 8 & 255;
        buf[pos + 2] = val >>> 16 & 255;
        buf[pos + 3] = val >>> 24;
      }
      Writer.prototype.fixed32 = function write_fixed32(value) {
        return this._push(writeFixed32, 4, value >>> 0);
      };
      Writer.prototype.sfixed32 = Writer.prototype.fixed32;
      Writer.prototype.fixed64 = function write_fixed64(value) {
        var bits = LongBits.from(value);
        return this._push(writeFixed32, 4, bits.lo)._push(writeFixed32, 4, bits.hi);
      };
      Writer.prototype.sfixed64 = Writer.prototype.fixed64;
      Writer.prototype.float = function write_float(value) {
        return this._push(util2.float.writeFloatLE, 4, value);
      };
      Writer.prototype.double = function write_double(value) {
        return this._push(util2.float.writeDoubleLE, 8, value);
      };
      var writeBytes = util2.Array.prototype.set ? function writeBytes_set(val, buf, pos) {
        buf.set(val, pos);
      } : function writeBytes_for(val, buf, pos) {
        for (var i = 0; i < val.length; ++i)
          buf[pos + i] = val[i];
      };
      Writer.prototype.bytes = function write_bytes(value) {
        var len = value.length >>> 0;
        if (!len)
          return this._push(writeByte, 1, 0);
        if (util2.isString(value)) {
          var buf = Writer.alloc(len = base64.length(value));
          base64.decode(value, buf, 0);
          value = buf;
        }
        return this.uint32(len)._push(writeBytes, len, value);
      };
      Writer.prototype.string = function write_string(value) {
        var len = utf8.length(value);
        return len ? this.uint32(len)._push(utf8.write, len, value) : this._push(writeByte, 1, 0);
      };
      Writer.prototype.fork = function fork() {
        this.states = new State(this);
        this.head = this.tail = new Op(noop, 0, 0);
        this.len = 0;
        return this;
      };
      Writer.prototype.reset = function reset() {
        if (this.states) {
          this.head = this.states.head;
          this.tail = this.states.tail;
          this.len = this.states.len;
          this.states = this.states.next;
        } else {
          this.head = this.tail = new Op(noop, 0, 0);
          this.len = 0;
        }
        return this;
      };
      Writer.prototype.ldelim = function ldelim() {
        var head = this.head, tail = this.tail, len = this.len;
        this.reset().uint32(len);
        if (len) {
          this.tail.next = head.next;
          this.tail = tail;
          this.len += len;
        }
        return this;
      };
      Writer.prototype.finish = function finish() {
        var head = this.head.next, buf = this.constructor.alloc(this.len), pos = 0;
        while (head) {
          head.fn(head.val, buf, pos);
          pos += head.len;
          head = head.next;
        }
        return buf;
      };
      Writer._configure = function(BufferWriter_) {
        BufferWriter = BufferWriter_;
        Writer.create = create();
        BufferWriter._configure();
      };
    }
  });

  // node_modules/protobufjs/src/writer_buffer.js
  var require_writer_buffer = __commonJS({
    "node_modules/protobufjs/src/writer_buffer.js"(exports, module) {
      "use strict";
      module.exports = BufferWriter;
      var Writer = require_writer();
      (BufferWriter.prototype = Object.create(Writer.prototype)).constructor = BufferWriter;
      var util2 = require_minimal();
      function BufferWriter() {
        Writer.call(this);
      }
      BufferWriter._configure = function() {
        BufferWriter.alloc = util2._Buffer_allocUnsafe;
        BufferWriter.writeBytesBuffer = util2.Buffer && util2.Buffer.prototype instanceof Uint8Array && util2.Buffer.prototype.set.name === "set" ? function writeBytesBuffer_set(val, buf, pos) {
          buf.set(val, pos);
        } : function writeBytesBuffer_copy(val, buf, pos) {
          if (val.copy)
            val.copy(buf, pos, 0, val.length);
          else for (var i = 0; i < val.length; )
            buf[pos++] = val[i++];
        };
      };
      BufferWriter.prototype.bytes = function write_bytes_buffer(value) {
        if (util2.isString(value))
          value = util2._Buffer_from(value, "base64");
        var len = value.length >>> 0;
        this.uint32(len);
        if (len)
          this._push(BufferWriter.writeBytesBuffer, len, value);
        return this;
      };
      function writeStringBuffer(val, buf, pos) {
        if (val.length < 40)
          util2.utf8.write(val, buf, pos);
        else if (buf.utf8Write)
          buf.utf8Write(val, pos);
        else
          buf.write(val, pos);
      }
      BufferWriter.prototype.string = function write_string_buffer(value) {
        var len = util2.Buffer.byteLength(value);
        this.uint32(len);
        if (len)
          this._push(writeStringBuffer, len, value);
        return this;
      };
      BufferWriter._configure();
    }
  });

  // node_modules/protobufjs/src/reader.js
  var require_reader = __commonJS({
    "node_modules/protobufjs/src/reader.js"(exports, module) {
      "use strict";
      module.exports = Reader;
      var util2 = require_minimal();
      var BufferReader;
      var LongBits = util2.LongBits;
      var utf8 = util2.utf8;
      function indexOutOfRange(reader, writeLength) {
        return RangeError("index out of range: " + reader.pos + " + " + (writeLength || 1) + " > " + reader.len);
      }
      function Reader(buffer) {
        this.buf = buffer;
        this.pos = 0;
        this.len = buffer.length;
      }
      var create_array = typeof Uint8Array !== "undefined" ? function create_typed_array(buffer) {
        if (buffer instanceof Uint8Array || Array.isArray(buffer))
          return new Reader(buffer);
        throw Error("illegal buffer");
      } : function create_array2(buffer) {
        if (Array.isArray(buffer))
          return new Reader(buffer);
        throw Error("illegal buffer");
      };
      var create = function create2() {
        return util2.Buffer ? function create_buffer_setup(buffer) {
          return (Reader.create = function create_buffer(buffer2) {
            return util2.Buffer.isBuffer(buffer2) ? new BufferReader(buffer2) : create_array(buffer2);
          })(buffer);
        } : create_array;
      };
      Reader.create = create();
      Reader.prototype._slice = util2.Array.prototype.subarray || /* istanbul ignore next */
      util2.Array.prototype.slice;
      Reader.prototype.uint32 = /* @__PURE__ */ function read_uint32_setup() {
        var value = 4294967295;
        return function read_uint32() {
          value = (this.buf[this.pos] & 127) >>> 0;
          if (this.buf[this.pos++] < 128) return value;
          value = (value | (this.buf[this.pos] & 127) << 7) >>> 0;
          if (this.buf[this.pos++] < 128) return value;
          value = (value | (this.buf[this.pos] & 127) << 14) >>> 0;
          if (this.buf[this.pos++] < 128) return value;
          value = (value | (this.buf[this.pos] & 127) << 21) >>> 0;
          if (this.buf[this.pos++] < 128) return value;
          value = (value | (this.buf[this.pos] & 15) << 28) >>> 0;
          if (this.buf[this.pos++] < 128) return value;
          if ((this.pos += 5) > this.len) {
            this.pos = this.len;
            throw indexOutOfRange(this, 10);
          }
          return value;
        };
      }();
      Reader.prototype.int32 = function read_int32() {
        return this.uint32() | 0;
      };
      Reader.prototype.sint32 = function read_sint32() {
        var value = this.uint32();
        return value >>> 1 ^ -(value & 1) | 0;
      };
      function readLongVarint() {
        var bits = new LongBits(0, 0);
        var i = 0;
        if (this.len - this.pos > 4) {
          for (; i < 4; ++i) {
            bits.lo = (bits.lo | (this.buf[this.pos] & 127) << i * 7) >>> 0;
            if (this.buf[this.pos++] < 128)
              return bits;
          }
          bits.lo = (bits.lo | (this.buf[this.pos] & 127) << 28) >>> 0;
          bits.hi = (bits.hi | (this.buf[this.pos] & 127) >> 4) >>> 0;
          if (this.buf[this.pos++] < 128)
            return bits;
          i = 0;
        } else {
          for (; i < 3; ++i) {
            if (this.pos >= this.len)
              throw indexOutOfRange(this);
            bits.lo = (bits.lo | (this.buf[this.pos] & 127) << i * 7) >>> 0;
            if (this.buf[this.pos++] < 128)
              return bits;
          }
          bits.lo = (bits.lo | (this.buf[this.pos++] & 127) << i * 7) >>> 0;
          return bits;
        }
        if (this.len - this.pos > 4) {
          for (; i < 5; ++i) {
            bits.hi = (bits.hi | (this.buf[this.pos] & 127) << i * 7 + 3) >>> 0;
            if (this.buf[this.pos++] < 128)
              return bits;
          }
        } else {
          for (; i < 5; ++i) {
            if (this.pos >= this.len)
              throw indexOutOfRange(this);
            bits.hi = (bits.hi | (this.buf[this.pos] & 127) << i * 7 + 3) >>> 0;
            if (this.buf[this.pos++] < 128)
              return bits;
          }
        }
        throw Error("invalid varint encoding");
      }
      Reader.prototype.bool = function read_bool() {
        return this.uint32() !== 0;
      };
      function readFixed32_end(buf, end) {
        return (buf[end - 4] | buf[end - 3] << 8 | buf[end - 2] << 16 | buf[end - 1] << 24) >>> 0;
      }
      Reader.prototype.fixed32 = function read_fixed32() {
        if (this.pos + 4 > this.len)
          throw indexOutOfRange(this, 4);
        return readFixed32_end(this.buf, this.pos += 4);
      };
      Reader.prototype.sfixed32 = function read_sfixed32() {
        if (this.pos + 4 > this.len)
          throw indexOutOfRange(this, 4);
        return readFixed32_end(this.buf, this.pos += 4) | 0;
      };
      function readFixed64() {
        if (this.pos + 8 > this.len)
          throw indexOutOfRange(this, 8);
        return new LongBits(readFixed32_end(this.buf, this.pos += 4), readFixed32_end(this.buf, this.pos += 4));
      }
      Reader.prototype.float = function read_float() {
        if (this.pos + 4 > this.len)
          throw indexOutOfRange(this, 4);
        var value = util2.float.readFloatLE(this.buf, this.pos);
        this.pos += 4;
        return value;
      };
      Reader.prototype.double = function read_double() {
        if (this.pos + 8 > this.len)
          throw indexOutOfRange(this, 4);
        var value = util2.float.readDoubleLE(this.buf, this.pos);
        this.pos += 8;
        return value;
      };
      Reader.prototype.bytes = function read_bytes() {
        var length = this.uint32(), start = this.pos, end = this.pos + length;
        if (end > this.len)
          throw indexOutOfRange(this, length);
        this.pos += length;
        if (Array.isArray(this.buf))
          return this.buf.slice(start, end);
        if (start === end) {
          var nativeBuffer = util2.Buffer;
          return nativeBuffer ? nativeBuffer.alloc(0) : new this.buf.constructor(0);
        }
        return this._slice.call(this.buf, start, end);
      };
      Reader.prototype.string = function read_string() {
        var bytes = this.bytes();
        return utf8.read(bytes, 0, bytes.length);
      };
      Reader.prototype.skip = function skip(length) {
        if (typeof length === "number") {
          if (this.pos + length > this.len)
            throw indexOutOfRange(this, length);
          this.pos += length;
        } else {
          do {
            if (this.pos >= this.len)
              throw indexOutOfRange(this);
          } while (this.buf[this.pos++] & 128);
        }
        return this;
      };
      Reader.prototype.skipType = function(wireType) {
        switch (wireType) {
          case 0:
            this.skip();
            break;
          case 1:
            this.skip(8);
            break;
          case 2:
            this.skip(this.uint32());
            break;
          case 3:
            while ((wireType = this.uint32() & 7) !== 4) {
              this.skipType(wireType);
            }
            break;
          case 5:
            this.skip(4);
            break;
          /* istanbul ignore next */
          default:
            throw Error("invalid wire type " + wireType + " at offset " + this.pos);
        }
        return this;
      };
      Reader._configure = function(BufferReader_) {
        BufferReader = BufferReader_;
        Reader.create = create();
        BufferReader._configure();
        var fn = util2.Long ? "toLong" : (
          /* istanbul ignore next */
          "toNumber"
        );
        util2.merge(Reader.prototype, {
          int64: function read_int64() {
            return readLongVarint.call(this)[fn](false);
          },
          uint64: function read_uint64() {
            return readLongVarint.call(this)[fn](true);
          },
          sint64: function read_sint64() {
            return readLongVarint.call(this).zzDecode()[fn](false);
          },
          fixed64: function read_fixed64() {
            return readFixed64.call(this)[fn](true);
          },
          sfixed64: function read_sfixed64() {
            return readFixed64.call(this)[fn](false);
          }
        });
      };
    }
  });

  // node_modules/protobufjs/src/reader_buffer.js
  var require_reader_buffer = __commonJS({
    "node_modules/protobufjs/src/reader_buffer.js"(exports, module) {
      "use strict";
      module.exports = BufferReader;
      var Reader = require_reader();
      (BufferReader.prototype = Object.create(Reader.prototype)).constructor = BufferReader;
      var util2 = require_minimal();
      function BufferReader(buffer) {
        Reader.call(this, buffer);
      }
      BufferReader._configure = function() {
        if (util2.Buffer)
          BufferReader.prototype._slice = util2.Buffer.prototype.slice;
      };
      BufferReader.prototype.string = function read_string_buffer() {
        var len = this.uint32();
        return this.buf.utf8Slice ? this.buf.utf8Slice(this.pos, this.pos = Math.min(this.pos + len, this.len)) : this.buf.toString("utf-8", this.pos, this.pos = Math.min(this.pos + len, this.len));
      };
      BufferReader._configure();
    }
  });

  // node_modules/protobufjs/src/rpc/service.js
  var require_service = __commonJS({
    "node_modules/protobufjs/src/rpc/service.js"(exports, module) {
      "use strict";
      module.exports = Service;
      var util2 = require_minimal();
      (Service.prototype = Object.create(util2.EventEmitter.prototype)).constructor = Service;
      function Service(rpcImpl, requestDelimited, responseDelimited) {
        if (typeof rpcImpl !== "function")
          throw TypeError("rpcImpl must be a function");
        util2.EventEmitter.call(this);
        this.rpcImpl = rpcImpl;
        this.requestDelimited = Boolean(requestDelimited);
        this.responseDelimited = Boolean(responseDelimited);
      }
      Service.prototype.rpcCall = function rpcCall(method, requestCtor, responseCtor, request, callback) {
        if (!request)
          throw TypeError("request must be specified");
        var self2 = this;
        if (!callback)
          return util2.asPromise(rpcCall, self2, method, requestCtor, responseCtor, request);
        if (!self2.rpcImpl) {
          setTimeout(function() {
            callback(Error("already ended"));
          }, 0);
          return void 0;
        }
        try {
          return self2.rpcImpl(
            method,
            requestCtor[self2.requestDelimited ? "encodeDelimited" : "encode"](request).finish(),
            function rpcCallback(err, response) {
              if (err) {
                self2.emit("error", err, method);
                return callback(err);
              }
              if (response === null) {
                self2.end(
                  /* endedByRPC */
                  true
                );
                return void 0;
              }
              if (!(response instanceof responseCtor)) {
                try {
                  response = responseCtor[self2.responseDelimited ? "decodeDelimited" : "decode"](response);
                } catch (err2) {
                  self2.emit("error", err2, method);
                  return callback(err2);
                }
              }
              self2.emit("data", response, method);
              return callback(null, response);
            }
          );
        } catch (err) {
          self2.emit("error", err, method);
          setTimeout(function() {
            callback(err);
          }, 0);
          return void 0;
        }
      };
      Service.prototype.end = function end(endedByRPC) {
        if (this.rpcImpl) {
          if (!endedByRPC)
            this.rpcImpl(null, null, null);
          this.rpcImpl = null;
          this.emit("end").off();
        }
        return this;
      };
    }
  });

  // node_modules/protobufjs/src/rpc.js
  var require_rpc = __commonJS({
    "node_modules/protobufjs/src/rpc.js"(exports) {
      "use strict";
      var rpc = exports;
      rpc.Service = require_service();
    }
  });

  // node_modules/protobufjs/src/roots.js
  var require_roots = __commonJS({
    "node_modules/protobufjs/src/roots.js"(exports, module) {
      "use strict";
      module.exports = {};
    }
  });

  // node_modules/protobufjs/src/index-minimal.js
  var require_index_minimal = __commonJS({
    "node_modules/protobufjs/src/index-minimal.js"(exports) {
      "use strict";
      var protobuf2 = exports;
      protobuf2.build = "minimal";
      protobuf2.Writer = require_writer();
      protobuf2.BufferWriter = require_writer_buffer();
      protobuf2.Reader = require_reader();
      protobuf2.BufferReader = require_reader_buffer();
      protobuf2.util = require_minimal();
      protobuf2.rpc = require_rpc();
      protobuf2.roots = require_roots();
      protobuf2.configure = configure2;
      function configure2() {
        protobuf2.util._configure();
        protobuf2.Writer._configure(protobuf2.BufferWriter);
        protobuf2.Reader._configure(protobuf2.BufferReader);
      }
      configure2();
    }
  });

  // node_modules/@protobufjs/codegen/index.js
  var require_codegen = __commonJS({
    "node_modules/@protobufjs/codegen/index.js"(exports, module) {
      "use strict";
      module.exports = codegen;
      var reservedRe = /^(?:do|if|in|for|let|new|try|var|case|else|enum|eval|false|null|this|true|void|with|break|catch|class|const|super|throw|while|yield|delete|export|import|public|return|static|switch|typeof|default|extends|finally|package|private|continue|debugger|function|arguments|interface|protected|implements|instanceof)$/;
      function codegen(functionParams, functionName) {
        if (typeof functionParams === "string") {
          functionName = functionParams;
          functionParams = void 0;
        }
        var body = [];
        function Codegen(formatStringOrScope) {
          if (typeof formatStringOrScope !== "string") {
            var source = toString2();
            if (codegen.verbose)
              console.log("codegen: " + source);
            source = "return " + source;
            if (formatStringOrScope) {
              var scopeKeys = Object.keys(formatStringOrScope), scopeParams = new Array(scopeKeys.length + 1), scopeValues = new Array(scopeKeys.length), scopeOffset = 0;
              while (scopeOffset < scopeKeys.length) {
                scopeParams[scopeOffset] = scopeKeys[scopeOffset];
                scopeValues[scopeOffset] = formatStringOrScope[scopeKeys[scopeOffset++]];
              }
              scopeParams[scopeOffset] = source;
              return Function.apply(null, scopeParams).apply(null, scopeValues);
            }
            return Function(source)();
          }
          var formatParams = new Array(arguments.length - 1), formatOffset = 0;
          while (formatOffset < formatParams.length)
            formatParams[formatOffset] = arguments[++formatOffset];
          formatOffset = 0;
          formatStringOrScope = formatStringOrScope.replace(/%([%dfijs])/g, function replace($0, $1) {
            var value = formatParams[formatOffset++];
            switch ($1) {
              case "d":
              case "f":
                return String(Number(value));
              case "i":
                return String(Math.floor(value));
              case "j":
                return JSON.stringify(value);
              case "s":
                return String(value);
            }
            return "%";
          });
          if (formatOffset !== formatParams.length)
            throw Error("parameter count mismatch");
          body.push(formatStringOrScope);
          return Codegen;
        }
        function toString2(functionNameOverride) {
          return "function " + safeFunctionName(functionNameOverride || functionName) + "(" + (functionParams && functionParams.join(",") || "") + "){\n  " + body.join("\n  ") + "\n}";
        }
        Codegen.toString = toString2;
        return Codegen;
      }
      codegen.verbose = false;
      function safeFunctionName(name) {
        if (!name)
          return "";
        name = String(name).replace(/[^\w$]/g, "");
        if (!name)
          return "";
        if (/^\d/.test(name))
          name = "_" + name;
        return reservedRe.test(name) ? name + "_" : name;
      }
    }
  });

  // (disabled):fs
  var require_fs = __commonJS({
    "(disabled):fs"() {
    }
  });

  // node_modules/@protobufjs/fetch/util/fs.js
  var require_fs2 = __commonJS({
    "node_modules/@protobufjs/fetch/util/fs.js"(exports, module) {
      "use strict";
      var fs = null;
      try {
        fs = require_fs();
        if (!fs || !fs.readFile || !fs.readFileSync)
          fs = null;
      } catch (e) {
      }
      module.exports = fs;
    }
  });

  // node_modules/@protobufjs/fetch/index.js
  var require_fetch = __commonJS({
    "node_modules/@protobufjs/fetch/index.js"(exports, module) {
      "use strict";
      module.exports = fetch;
      var asPromise = require_aspromise();
      var fs = require_fs2();
      function fetch(filename, options, callback) {
        if (typeof options === "function") {
          callback = options;
          options = {};
        } else if (!options)
          options = {};
        if (!callback)
          return asPromise(fetch, this, filename, options);
        if (!options.xhr && fs && fs.readFile)
          return fs.readFile(filename, function fetchReadFileCallback(err, contents) {
            return err && typeof XMLHttpRequest !== "undefined" ? fetch.xhr(filename, options, callback) : err ? callback(err) : callback(null, options.binary ? contents : contents.toString("utf8"));
          });
        return fetch.xhr(filename, options, callback);
      }
      fetch.xhr = function fetch_xhr(filename, options, callback) {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function fetchOnReadyStateChange() {
          if (xhr.readyState !== 4)
            return void 0;
          if (xhr.status !== 0 && xhr.status !== 200)
            return callback(Error("status " + xhr.status));
          if (options.binary) {
            var buffer = xhr.response;
            if (!buffer) {
              buffer = [];
              for (var i = 0; i < xhr.responseText.length; ++i)
                buffer.push(xhr.responseText.charCodeAt(i) & 255);
            }
            return callback(null, typeof Uint8Array !== "undefined" ? new Uint8Array(buffer) : buffer);
          }
          return callback(null, xhr.responseText);
        };
        if (options.binary) {
          if ("overrideMimeType" in xhr)
            xhr.overrideMimeType("text/plain; charset=x-user-defined");
          xhr.responseType = "arraybuffer";
        }
        xhr.open("GET", filename);
        xhr.send();
      };
    }
  });

  // node_modules/@protobufjs/path/index.js
  var require_path = __commonJS({
    "node_modules/@protobufjs/path/index.js"(exports) {
      "use strict";
      var path = exports;
      var isAbsolute = (
        /**
         * Tests if the specified path is absolute.
         * @param {string} path Path to test
         * @returns {boolean} `true` if path is absolute
         */
        path.isAbsolute = function isAbsolute2(path2) {
          return /^(?:\/|\w+:)/.test(path2);
        }
      );
      var normalize = (
        /**
         * Normalizes the specified path.
         * @param {string} path Path to normalize
         * @returns {string} Normalized path
         */
        path.normalize = function normalize2(path2) {
          path2 = path2.replace(/\\/g, "/").replace(/\/{2,}/g, "/");
          var parts = path2.split("/"), absolute = isAbsolute(path2), prefix = "";
          if (absolute)
            prefix = parts.shift() + "/";
          for (var i = 0; i < parts.length; ) {
            if (parts[i] === "..") {
              if (i > 0 && parts[i - 1] !== "..")
                parts.splice(--i, 2);
              else if (absolute)
                parts.splice(i, 1);
              else
                ++i;
            } else if (parts[i] === ".")
              parts.splice(i, 1);
            else
              ++i;
          }
          return prefix + parts.join("/");
        }
      );
      path.resolve = function resolve(originPath, includePath, alreadyNormalized) {
        if (!alreadyNormalized)
          includePath = normalize(includePath);
        if (isAbsolute(includePath))
          return includePath;
        if (!alreadyNormalized)
          originPath = normalize(originPath);
        return (originPath = originPath.replace(/(?:\/|^)[^/]+$/, "")).length ? normalize(originPath + "/" + includePath) : includePath;
      };
    }
  });

  // node_modules/protobufjs/src/namespace.js
  var require_namespace = __commonJS({
    "node_modules/protobufjs/src/namespace.js"(exports, module) {
      "use strict";
      module.exports = Namespace;
      var ReflectionObject = require_object();
      ((Namespace.prototype = Object.create(ReflectionObject.prototype)).constructor = Namespace).className = "Namespace";
      var Field = require_field();
      var util2 = require_util();
      var OneOf = require_oneof();
      var Type;
      var Service;
      var Enum;
      Namespace.fromJSON = function fromJSON(name, json) {
        return new Namespace(name, json.options).addJSON(json.nested);
      };
      function arrayToJSON(array, toJSONOptions) {
        if (!(array && array.length))
          return void 0;
        var obj = {};
        for (var i = 0; i < array.length; ++i)
          obj[array[i].name] = array[i].toJSON(toJSONOptions);
        return obj;
      }
      Namespace.arrayToJSON = arrayToJSON;
      Namespace.isReservedId = function isReservedId(reserved, id) {
        if (reserved) {
          for (var i = 0; i < reserved.length; ++i)
            if (typeof reserved[i] !== "string" && reserved[i][0] <= id && reserved[i][1] > id)
              return true;
        }
        return false;
      };
      Namespace.isReservedName = function isReservedName(reserved, name) {
        if (reserved) {
          for (var i = 0; i < reserved.length; ++i)
            if (reserved[i] === name)
              return true;
        }
        return false;
      };
      function Namespace(name, options) {
        ReflectionObject.call(this, name, options);
        this.nested = void 0;
        this._nestedArray = null;
        this._lookupCache = {};
        this._needsRecursiveFeatureResolution = true;
        this._needsRecursiveResolve = true;
      }
      function clearCache(namespace) {
        namespace._nestedArray = null;
        namespace._lookupCache = {};
        var parent = namespace;
        while (parent = parent.parent) {
          parent._lookupCache = {};
        }
        return namespace;
      }
      Object.defineProperty(Namespace.prototype, "nestedArray", {
        get: function() {
          return this._nestedArray || (this._nestedArray = util2.toArray(this.nested));
        }
      });
      Namespace.prototype.toJSON = function toJSON(toJSONOptions) {
        return util2.toObject([
          "options",
          this.options,
          "nested",
          arrayToJSON(this.nestedArray, toJSONOptions)
        ]);
      };
      Namespace.prototype.addJSON = function addJSON(nestedJson) {
        var ns = this;
        if (nestedJson) {
          for (var names = Object.keys(nestedJson), i = 0, nested; i < names.length; ++i) {
            nested = nestedJson[names[i]];
            ns.add(
              // most to least likely
              (nested.fields !== void 0 ? Type.fromJSON : nested.values !== void 0 ? Enum.fromJSON : nested.methods !== void 0 ? Service.fromJSON : nested.id !== void 0 ? Field.fromJSON : Namespace.fromJSON)(names[i], nested)
            );
          }
        }
        return this;
      };
      Namespace.prototype.get = function get(name) {
        return this.nested && this.nested[name] || null;
      };
      Namespace.prototype.getEnum = function getEnum(name) {
        if (this.nested && this.nested[name] instanceof Enum)
          return this.nested[name].values;
        throw Error("no such enum: " + name);
      };
      Namespace.prototype.add = function add2(object) {
        if (!(object instanceof Field && object.extend !== void 0 || object instanceof Type || object instanceof OneOf || object instanceof Enum || object instanceof Service || object instanceof Namespace))
          throw TypeError("object must be a valid nested object");
        if (!this.nested)
          this.nested = {};
        else {
          var prev = this.get(object.name);
          if (prev) {
            if (prev instanceof Namespace && object instanceof Namespace && !(prev instanceof Type || prev instanceof Service)) {
              var nested = prev.nestedArray;
              for (var i = 0; i < nested.length; ++i)
                object.add(nested[i]);
              this.remove(prev);
              if (!this.nested)
                this.nested = {};
              object.setOptions(prev.options, true);
            } else
              throw Error("duplicate name '" + object.name + "' in " + this);
          }
        }
        this.nested[object.name] = object;
        if (!(this instanceof Type || this instanceof Service || this instanceof Enum || this instanceof Field)) {
          if (!object._edition) {
            object._edition = object._defaultEdition;
          }
        }
        this._needsRecursiveFeatureResolution = true;
        this._needsRecursiveResolve = true;
        var parent = this;
        while (parent = parent.parent) {
          parent._needsRecursiveFeatureResolution = true;
          parent._needsRecursiveResolve = true;
        }
        object.onAdd(this);
        return clearCache(this);
      };
      Namespace.prototype.remove = function remove(object) {
        if (!(object instanceof ReflectionObject))
          throw TypeError("object must be a ReflectionObject");
        if (object.parent !== this)
          throw Error(object + " is not a member of " + this);
        delete this.nested[object.name];
        if (!Object.keys(this.nested).length)
          this.nested = void 0;
        object.onRemove(this);
        return clearCache(this);
      };
      Namespace.prototype.define = function define(path, json) {
        if (util2.isString(path))
          path = path.split(".");
        else if (!Array.isArray(path))
          throw TypeError("illegal path");
        if (path && path.length && path[0] === "")
          throw Error("path must be relative");
        var ptr = this;
        while (path.length > 0) {
          var part = path.shift();
          if (ptr.nested && ptr.nested[part]) {
            ptr = ptr.nested[part];
            if (!(ptr instanceof Namespace))
              throw Error("path conflicts with non-namespace objects");
          } else
            ptr.add(ptr = new Namespace(part));
        }
        if (json)
          ptr.addJSON(json);
        return ptr;
      };
      Namespace.prototype.resolveAll = function resolveAll() {
        if (!this._needsRecursiveResolve) return this;
        this._resolveFeaturesRecursive(this._edition);
        var nested = this.nestedArray, i = 0;
        this.resolve();
        while (i < nested.length)
          if (nested[i] instanceof Namespace)
            nested[i++].resolveAll();
          else
            nested[i++].resolve();
        this._needsRecursiveResolve = false;
        return this;
      };
      Namespace.prototype._resolveFeaturesRecursive = function _resolveFeaturesRecursive(edition) {
        if (!this._needsRecursiveFeatureResolution) return this;
        this._needsRecursiveFeatureResolution = false;
        edition = this._edition || edition;
        ReflectionObject.prototype._resolveFeaturesRecursive.call(this, edition);
        this.nestedArray.forEach((nested) => {
          nested._resolveFeaturesRecursive(edition);
        });
        return this;
      };
      Namespace.prototype.lookup = function lookup2(path, filterTypes, parentAlreadyChecked) {
        if (typeof filterTypes === "boolean") {
          parentAlreadyChecked = filterTypes;
          filterTypes = void 0;
        } else if (filterTypes && !Array.isArray(filterTypes))
          filterTypes = [filterTypes];
        if (util2.isString(path) && path.length) {
          if (path === ".")
            return this.root;
          path = path.split(".");
        } else if (!path.length)
          return this;
        var flatPath = path.join(".");
        if (path[0] === "")
          return this.root.lookup(path.slice(1), filterTypes);
        var found = this.root._fullyQualifiedObjects && this.root._fullyQualifiedObjects["." + flatPath];
        if (found && (!filterTypes || filterTypes.indexOf(found.constructor) > -1)) {
          return found;
        }
        found = this._lookupImpl(path, flatPath);
        if (found && (!filterTypes || filterTypes.indexOf(found.constructor) > -1)) {
          return found;
        }
        if (parentAlreadyChecked)
          return null;
        var current = this;
        while (current.parent) {
          found = current.parent._lookupImpl(path, flatPath);
          if (found && (!filterTypes || filterTypes.indexOf(found.constructor) > -1)) {
            return found;
          }
          current = current.parent;
        }
        return null;
      };
      Namespace.prototype._lookupImpl = function lookup2(path, flatPath) {
        if (Object.prototype.hasOwnProperty.call(this._lookupCache, flatPath)) {
          return this._lookupCache[flatPath];
        }
        var found = this.get(path[0]);
        var exact = null;
        if (found) {
          if (path.length === 1) {
            exact = found;
          } else if (found instanceof Namespace) {
            path = path.slice(1);
            exact = found._lookupImpl(path, path.join("."));
          }
        } else {
          for (var i = 0; i < this.nestedArray.length; ++i)
            if (this._nestedArray[i] instanceof Namespace && (found = this._nestedArray[i]._lookupImpl(path, flatPath)))
              exact = found;
        }
        this._lookupCache[flatPath] = exact;
        return exact;
      };
      Namespace.prototype.lookupType = function lookupType(path) {
        var found = this.lookup(path, [Type]);
        if (!found)
          throw Error("no such type: " + path);
        return found;
      };
      Namespace.prototype.lookupEnum = function lookupEnum(path) {
        var found = this.lookup(path, [Enum]);
        if (!found)
          throw Error("no such Enum '" + path + "' in " + this);
        return found;
      };
      Namespace.prototype.lookupTypeOrEnum = function lookupTypeOrEnum(path) {
        var found = this.lookup(path, [Type, Enum]);
        if (!found)
          throw Error("no such Type or Enum '" + path + "' in " + this);
        return found;
      };
      Namespace.prototype.lookupService = function lookupService(path) {
        var found = this.lookup(path, [Service]);
        if (!found)
          throw Error("no such Service '" + path + "' in " + this);
        return found;
      };
      Namespace._configure = function(Type_, Service_, Enum_) {
        Type = Type_;
        Service = Service_;
        Enum = Enum_;
      };
    }
  });

  // node_modules/protobufjs/src/mapfield.js
  var require_mapfield = __commonJS({
    "node_modules/protobufjs/src/mapfield.js"(exports, module) {
      "use strict";
      module.exports = MapField;
      var Field = require_field();
      ((MapField.prototype = Object.create(Field.prototype)).constructor = MapField).className = "MapField";
      var types = require_types();
      var util2 = require_util();
      function MapField(name, id, keyType, type, options, comment) {
        Field.call(this, name, id, type, void 0, void 0, options, comment);
        if (!util2.isString(keyType))
          throw TypeError("keyType must be a string");
        this.keyType = keyType;
        this.resolvedKeyType = null;
        this.map = true;
      }
      MapField.fromJSON = function fromJSON(name, json) {
        return new MapField(name, json.id, json.keyType, json.type, json.options, json.comment);
      };
      MapField.prototype.toJSON = function toJSON(toJSONOptions) {
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "keyType",
          this.keyType,
          "type",
          this.type,
          "id",
          this.id,
          "extend",
          this.extend,
          "options",
          this.options,
          "comment",
          keepComments ? this.comment : void 0
        ]);
      };
      MapField.prototype.resolve = function resolve() {
        if (this.resolved)
          return this;
        if (types.mapKey[this.keyType] === void 0)
          throw Error("invalid key type: " + this.keyType);
        return Field.prototype.resolve.call(this);
      };
      MapField.d = function decorateMapField(fieldId, fieldKeyType, fieldValueType) {
        if (typeof fieldValueType === "function")
          fieldValueType = util2.decorateType(fieldValueType).name;
        else if (fieldValueType && typeof fieldValueType === "object")
          fieldValueType = util2.decorateEnum(fieldValueType).name;
        return function mapFieldDecorator(prototype, fieldName) {
          util2.decorateType(prototype.constructor).add(new MapField(fieldName, fieldId, fieldKeyType, fieldValueType));
        };
      };
    }
  });

  // node_modules/protobufjs/src/method.js
  var require_method = __commonJS({
    "node_modules/protobufjs/src/method.js"(exports, module) {
      "use strict";
      module.exports = Method;
      var ReflectionObject = require_object();
      ((Method.prototype = Object.create(ReflectionObject.prototype)).constructor = Method).className = "Method";
      var util2 = require_util();
      function Method(name, type, requestType, responseType, requestStream, responseStream, options, comment, parsedOptions) {
        if (util2.isObject(requestStream)) {
          options = requestStream;
          requestStream = responseStream = void 0;
        } else if (util2.isObject(responseStream)) {
          options = responseStream;
          responseStream = void 0;
        }
        if (!(type === void 0 || util2.isString(type)))
          throw TypeError("type must be a string");
        if (!util2.isString(requestType))
          throw TypeError("requestType must be a string");
        if (!util2.isString(responseType))
          throw TypeError("responseType must be a string");
        ReflectionObject.call(this, name, options);
        this.type = type || "rpc";
        this.requestType = requestType;
        this.requestStream = requestStream ? true : void 0;
        this.responseType = responseType;
        this.responseStream = responseStream ? true : void 0;
        this.resolvedRequestType = null;
        this.resolvedResponseType = null;
        this.comment = comment;
        this.parsedOptions = parsedOptions;
      }
      Method.fromJSON = function fromJSON(name, json) {
        return new Method(name, json.type, json.requestType, json.responseType, json.requestStream, json.responseStream, json.options, json.comment, json.parsedOptions);
      };
      Method.prototype.toJSON = function toJSON(toJSONOptions) {
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "type",
          this.type !== "rpc" && /* istanbul ignore next */
          this.type || void 0,
          "requestType",
          this.requestType,
          "requestStream",
          this.requestStream,
          "responseType",
          this.responseType,
          "responseStream",
          this.responseStream,
          "options",
          this.options,
          "comment",
          keepComments ? this.comment : void 0,
          "parsedOptions",
          this.parsedOptions
        ]);
      };
      Method.prototype.resolve = function resolve() {
        if (this.resolved)
          return this;
        this.resolvedRequestType = this.parent.lookupType(this.requestType);
        this.resolvedResponseType = this.parent.lookupType(this.responseType);
        return ReflectionObject.prototype.resolve.call(this);
      };
    }
  });

  // node_modules/protobufjs/src/service.js
  var require_service2 = __commonJS({
    "node_modules/protobufjs/src/service.js"(exports, module) {
      "use strict";
      module.exports = Service;
      var Namespace = require_namespace();
      ((Service.prototype = Object.create(Namespace.prototype)).constructor = Service).className = "Service";
      var Method = require_method();
      var util2 = require_util();
      var rpc = require_rpc();
      function Service(name, options) {
        Namespace.call(this, name, options);
        this.methods = {};
        this._methodsArray = null;
      }
      Service.fromJSON = function fromJSON(name, json) {
        var service = new Service(name, json.options);
        if (json.methods)
          for (var names = Object.keys(json.methods), i = 0; i < names.length; ++i)
            service.add(Method.fromJSON(names[i], json.methods[names[i]]));
        if (json.nested)
          service.addJSON(json.nested);
        if (json.edition)
          service._edition = json.edition;
        service.comment = json.comment;
        service._defaultEdition = "proto3";
        return service;
      };
      Service.prototype.toJSON = function toJSON(toJSONOptions) {
        var inherited = Namespace.prototype.toJSON.call(this, toJSONOptions);
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "edition",
          this._editionToJSON(),
          "options",
          inherited && inherited.options || void 0,
          "methods",
          Namespace.arrayToJSON(this.methodsArray, toJSONOptions) || /* istanbul ignore next */
          {},
          "nested",
          inherited && inherited.nested || void 0,
          "comment",
          keepComments ? this.comment : void 0
        ]);
      };
      Object.defineProperty(Service.prototype, "methodsArray", {
        get: function() {
          return this._methodsArray || (this._methodsArray = util2.toArray(this.methods));
        }
      });
      function clearCache(service) {
        service._methodsArray = null;
        return service;
      }
      Service.prototype.get = function get(name) {
        return this.methods[name] || Namespace.prototype.get.call(this, name);
      };
      Service.prototype.resolveAll = function resolveAll() {
        if (!this._needsRecursiveResolve) return this;
        Namespace.prototype.resolve.call(this);
        var methods = this.methodsArray;
        for (var i = 0; i < methods.length; ++i)
          methods[i].resolve();
        return this;
      };
      Service.prototype._resolveFeaturesRecursive = function _resolveFeaturesRecursive(edition) {
        if (!this._needsRecursiveFeatureResolution) return this;
        edition = this._edition || edition;
        Namespace.prototype._resolveFeaturesRecursive.call(this, edition);
        this.methodsArray.forEach((method) => {
          method._resolveFeaturesRecursive(edition);
        });
        return this;
      };
      Service.prototype.add = function add2(object) {
        if (this.get(object.name))
          throw Error("duplicate name '" + object.name + "' in " + this);
        if (object instanceof Method) {
          this.methods[object.name] = object;
          object.parent = this;
          return clearCache(this);
        }
        return Namespace.prototype.add.call(this, object);
      };
      Service.prototype.remove = function remove(object) {
        if (object instanceof Method) {
          if (this.methods[object.name] !== object)
            throw Error(object + " is not a member of " + this);
          delete this.methods[object.name];
          object.parent = null;
          return clearCache(this);
        }
        return Namespace.prototype.remove.call(this, object);
      };
      Service.prototype.create = function create(rpcImpl, requestDelimited, responseDelimited) {
        var rpcService = new rpc.Service(rpcImpl, requestDelimited, responseDelimited);
        for (var i = 0, method; i < /* initializes */
        this.methodsArray.length; ++i) {
          var methodName = util2.lcFirst((method = this._methodsArray[i]).resolve().name).replace(/[^$\w_]/g, "");
          rpcService[methodName] = util2.codegen(["r", "c"], util2.isReserved(methodName) ? methodName + "_" : methodName)("return this.rpcCall(m,q,s,r,c)")({
            m: method,
            q: method.resolvedRequestType.ctor,
            s: method.resolvedResponseType.ctor
          });
        }
        return rpcService;
      };
    }
  });

  // node_modules/protobufjs/src/message.js
  var require_message = __commonJS({
    "node_modules/protobufjs/src/message.js"(exports, module) {
      "use strict";
      module.exports = Message2;
      var util2 = require_minimal();
      function Message2(properties) {
        if (properties)
          for (var keys = Object.keys(properties), i = 0; i < keys.length; ++i)
            this[keys[i]] = properties[keys[i]];
      }
      Message2.create = function create(properties) {
        return this.$type.create(properties);
      };
      Message2.encode = function encode(message, writer) {
        return this.$type.encode(message, writer);
      };
      Message2.encodeDelimited = function encodeDelimited(message, writer) {
        return this.$type.encodeDelimited(message, writer);
      };
      Message2.decode = function decode(reader) {
        return this.$type.decode(reader);
      };
      Message2.decodeDelimited = function decodeDelimited(reader) {
        return this.$type.decodeDelimited(reader);
      };
      Message2.verify = function verify(message) {
        return this.$type.verify(message);
      };
      Message2.fromObject = function fromObject(object) {
        return this.$type.fromObject(object);
      };
      Message2.toObject = function toObject(message, options) {
        return this.$type.toObject(message, options);
      };
      Message2.prototype.toJSON = function toJSON() {
        return this.$type.toObject(this, util2.toJSONOptions);
      };
    }
  });

  // node_modules/protobufjs/src/decoder.js
  var require_decoder = __commonJS({
    "node_modules/protobufjs/src/decoder.js"(exports, module) {
      "use strict";
      module.exports = decoder;
      var Enum = require_enum();
      var types = require_types();
      var util2 = require_util();
      function missing(field) {
        return "missing required '" + field.name + "'";
      }
      function decoder(mtype) {
        var gen = util2.codegen(["r", "l", "e"], mtype.name + "$decode")("if(!(r instanceof Reader))")("r=Reader.create(r)")("var c=l===undefined?r.len:r.pos+l,m=new this.ctor" + (mtype.fieldsArray.filter(function(field2) {
          return field2.map;
        }).length ? ",k,value" : ""))("while(r.pos<c){")("var t=r.uint32()")("if(t===e)")("break")("switch(t>>>3){");
        var i = 0;
        for (; i < /* initializes */
        mtype.fieldsArray.length; ++i) {
          var field = mtype._fieldsArray[i].resolve(), type = field.resolvedType instanceof Enum ? "int32" : field.type, ref = "m" + util2.safeProp(field.name);
          gen("case %i: {", field.id);
          if (field.map) {
            gen("if(%s===util.emptyObject)", ref)("%s={}", ref)("var c2 = r.uint32()+r.pos");
            if (types.defaults[field.keyType] !== void 0) gen("k=%j", types.defaults[field.keyType]);
            else gen("k=null");
            if (types.defaults[type] !== void 0) gen("value=%j", types.defaults[type]);
            else gen("value=null");
            gen("while(r.pos<c2){")("var tag2=r.uint32()")("switch(tag2>>>3){")("case 1: k=r.%s(); break", field.keyType)("case 2:");
            if (types.basic[type] === void 0) gen("value=types[%i].decode(r,r.uint32())", i);
            else gen("value=r.%s()", type);
            gen("break")("default:")("r.skipType(tag2&7)")("break")("}")("}");
            if (types.long[field.keyType] !== void 0) gen('%s[typeof k==="object"?util.longToHash(k):k]=value', ref);
            else gen("%s[k]=value", ref);
          } else if (field.repeated) {
            gen("if(!(%s&&%s.length))", ref, ref)("%s=[]", ref);
            if (types.packed[type] !== void 0) gen("if((t&7)===2){")("var c2=r.uint32()+r.pos")("while(r.pos<c2)")("%s.push(r.%s())", ref, type)("}else");
            if (types.basic[type] === void 0) gen(field.delimited ? "%s.push(types[%i].decode(r,undefined,((t&~7)|4)))" : "%s.push(types[%i].decode(r,r.uint32()))", ref, i);
            else gen("%s.push(r.%s())", ref, type);
          } else if (types.basic[type] === void 0) gen(field.delimited ? "%s=types[%i].decode(r,undefined,((t&~7)|4))" : "%s=types[%i].decode(r,r.uint32())", ref, i);
          else gen("%s=r.%s()", ref, type);
          gen("break")("}");
        }
        gen("default:")("r.skipType(t&7)")("break")("}")("}");
        for (i = 0; i < mtype._fieldsArray.length; ++i) {
          var rfield = mtype._fieldsArray[i];
          if (rfield.required) gen("if(!m.hasOwnProperty(%j))", rfield.name)("throw util.ProtocolError(%j,{instance:m})", missing(rfield));
        }
        return gen("return m");
      }
    }
  });

  // node_modules/protobufjs/src/verifier.js
  var require_verifier = __commonJS({
    "node_modules/protobufjs/src/verifier.js"(exports, module) {
      "use strict";
      module.exports = verifier;
      var Enum = require_enum();
      var util2 = require_util();
      function invalid(field, expected) {
        return field.name + ": " + expected + (field.repeated && expected !== "array" ? "[]" : field.map && expected !== "object" ? "{k:" + field.keyType + "}" : "") + " expected";
      }
      function genVerifyValue(gen, field, fieldIndex, ref) {
        if (field.resolvedType) {
          if (field.resolvedType instanceof Enum) {
            gen("switch(%s){", ref)("default:")("return%j", invalid(field, "enum value"));
            for (var keys = Object.keys(field.resolvedType.values), j = 0; j < keys.length; ++j) gen("case %i:", field.resolvedType.values[keys[j]]);
            gen("break")("}");
          } else {
            gen("{")("var e=types[%i].verify(%s);", fieldIndex, ref)("if(e)")("return%j+e", field.name + ".")("}");
          }
        } else {
          switch (field.type) {
            case "int32":
            case "uint32":
            case "sint32":
            case "fixed32":
            case "sfixed32":
              gen("if(!util.isInteger(%s))", ref)("return%j", invalid(field, "integer"));
              break;
            case "int64":
            case "uint64":
            case "sint64":
            case "fixed64":
            case "sfixed64":
              gen("if(!util.isInteger(%s)&&!(%s&&util.isInteger(%s.low)&&util.isInteger(%s.high)))", ref, ref, ref, ref)("return%j", invalid(field, "integer|Long"));
              break;
            case "float":
            case "double":
              gen('if(typeof %s!=="number")', ref)("return%j", invalid(field, "number"));
              break;
            case "bool":
              gen('if(typeof %s!=="boolean")', ref)("return%j", invalid(field, "boolean"));
              break;
            case "string":
              gen("if(!util.isString(%s))", ref)("return%j", invalid(field, "string"));
              break;
            case "bytes":
              gen('if(!(%s&&typeof %s.length==="number"||util.isString(%s)))', ref, ref, ref)("return%j", invalid(field, "buffer"));
              break;
          }
        }
        return gen;
      }
      function genVerifyKey(gen, field, ref) {
        switch (field.keyType) {
          case "int32":
          case "uint32":
          case "sint32":
          case "fixed32":
          case "sfixed32":
            gen("if(!util.key32Re.test(%s))", ref)("return%j", invalid(field, "integer key"));
            break;
          case "int64":
          case "uint64":
          case "sint64":
          case "fixed64":
          case "sfixed64":
            gen("if(!util.key64Re.test(%s))", ref)("return%j", invalid(field, "integer|Long key"));
            break;
          case "bool":
            gen("if(!util.key2Re.test(%s))", ref)("return%j", invalid(field, "boolean key"));
            break;
        }
        return gen;
      }
      function verifier(mtype) {
        var gen = util2.codegen(["m"], mtype.name + "$verify")('if(typeof m!=="object"||m===null)')("return%j", "object expected");
        var oneofs = mtype.oneofsArray, seenFirstField = {};
        if (oneofs.length) gen("var p={}");
        for (var i = 0; i < /* initializes */
        mtype.fieldsArray.length; ++i) {
          var field = mtype._fieldsArray[i].resolve(), ref = "m" + util2.safeProp(field.name);
          if (field.optional) gen("if(%s!=null&&m.hasOwnProperty(%j)){", ref, field.name);
          if (field.map) {
            gen("if(!util.isObject(%s))", ref)("return%j", invalid(field, "object"))("var k=Object.keys(%s)", ref)("for(var i=0;i<k.length;++i){");
            genVerifyKey(gen, field, "k[i]");
            genVerifyValue(gen, field, i, ref + "[k[i]]")("}");
          } else if (field.repeated) {
            gen("if(!Array.isArray(%s))", ref)("return%j", invalid(field, "array"))("for(var i=0;i<%s.length;++i){", ref);
            genVerifyValue(gen, field, i, ref + "[i]")("}");
          } else {
            if (field.partOf) {
              var oneofProp = util2.safeProp(field.partOf.name);
              if (seenFirstField[field.partOf.name] === 1) gen("if(p%s===1)", oneofProp)("return%j", field.partOf.name + ": multiple values");
              seenFirstField[field.partOf.name] = 1;
              gen("p%s=1", oneofProp);
            }
            genVerifyValue(gen, field, i, ref);
          }
          if (field.optional) gen("}");
        }
        return gen("return null");
      }
    }
  });

  // node_modules/protobufjs/src/converter.js
  var require_converter = __commonJS({
    "node_modules/protobufjs/src/converter.js"(exports) {
      "use strict";
      var converter = exports;
      var Enum = require_enum();
      var util2 = require_util();
      function genValuePartial_fromObject(gen, field, fieldIndex, prop) {
        var defaultAlreadyEmitted = false;
        if (field.resolvedType) {
          if (field.resolvedType instanceof Enum) {
            gen("switch(d%s){", prop);
            for (var values = field.resolvedType.values, keys = Object.keys(values), i = 0; i < keys.length; ++i) {
              if (values[keys[i]] === field.typeDefault && !defaultAlreadyEmitted) {
                gen("default:")('if(typeof(d%s)==="number"){m%s=d%s;break}', prop, prop, prop);
                if (!field.repeated) gen("break");
                defaultAlreadyEmitted = true;
              }
              gen("case%j:", keys[i])("case %i:", values[keys[i]])("m%s=%j", prop, values[keys[i]])("break");
            }
            gen("}");
          } else gen('if(typeof d%s!=="object")', prop)("throw TypeError(%j)", field.fullName + ": object expected")("m%s=types[%i].fromObject(d%s)", prop, fieldIndex, prop);
        } else {
          var isUnsigned = false;
          switch (field.type) {
            case "double":
            case "float":
              gen("m%s=Number(d%s)", prop, prop);
              break;
            case "uint32":
            case "fixed32":
              gen("m%s=d%s>>>0", prop, prop);
              break;
            case "int32":
            case "sint32":
            case "sfixed32":
              gen("m%s=d%s|0", prop, prop);
              break;
            case "uint64":
              isUnsigned = true;
            // eslint-disable-next-line no-fallthrough
            case "int64":
            case "sint64":
            case "fixed64":
            case "sfixed64":
              gen("if(util.Long)")("(m%s=util.Long.fromValue(d%s)).unsigned=%j", prop, prop, isUnsigned)('else if(typeof d%s==="string")', prop)("m%s=parseInt(d%s,10)", prop, prop)('else if(typeof d%s==="number")', prop)("m%s=d%s", prop, prop)('else if(typeof d%s==="object")', prop)("m%s=new util.LongBits(d%s.low>>>0,d%s.high>>>0).toNumber(%s)", prop, prop, prop, isUnsigned ? "true" : "");
              break;
            case "bytes":
              gen('if(typeof d%s==="string")', prop)("util.base64.decode(d%s,m%s=util.newBuffer(util.base64.length(d%s)),0)", prop, prop, prop)("else if(d%s.length >= 0)", prop)("m%s=d%s", prop, prop);
              break;
            case "string":
              gen("m%s=String(d%s)", prop, prop);
              break;
            case "bool":
              gen("m%s=Boolean(d%s)", prop, prop);
              break;
          }
        }
        return gen;
      }
      converter.fromObject = function fromObject(mtype) {
        var fields = mtype.fieldsArray;
        var gen = util2.codegen(["d"], mtype.name + "$fromObject")("if(d instanceof this.ctor)")("return d");
        if (!fields.length) return gen("return new this.ctor");
        gen("var m=new this.ctor");
        for (var i = 0; i < fields.length; ++i) {
          var field = fields[i].resolve(), prop = util2.safeProp(field.name);
          if (field.map) {
            gen("if(d%s){", prop)('if(typeof d%s!=="object")', prop)("throw TypeError(%j)", field.fullName + ": object expected")("m%s={}", prop)("for(var ks=Object.keys(d%s),i=0;i<ks.length;++i){", prop);
            genValuePartial_fromObject(
              gen,
              field,
              /* not sorted */
              i,
              prop + "[ks[i]]"
            )("}")("}");
          } else if (field.repeated) {
            gen("if(d%s){", prop)("if(!Array.isArray(d%s))", prop)("throw TypeError(%j)", field.fullName + ": array expected")("m%s=[]", prop)("for(var i=0;i<d%s.length;++i){", prop);
            genValuePartial_fromObject(
              gen,
              field,
              /* not sorted */
              i,
              prop + "[i]"
            )("}")("}");
          } else {
            if (!(field.resolvedType instanceof Enum)) gen("if(d%s!=null){", prop);
            genValuePartial_fromObject(
              gen,
              field,
              /* not sorted */
              i,
              prop
            );
            if (!(field.resolvedType instanceof Enum)) gen("}");
          }
        }
        return gen("return m");
      };
      function genValuePartial_toObject(gen, field, fieldIndex, prop) {
        if (field.resolvedType) {
          if (field.resolvedType instanceof Enum) gen("d%s=o.enums===String?(types[%i].values[m%s]===undefined?m%s:types[%i].values[m%s]):m%s", prop, fieldIndex, prop, prop, fieldIndex, prop, prop);
          else gen("d%s=types[%i].toObject(m%s,o)", prop, fieldIndex, prop);
        } else {
          var isUnsigned = false;
          switch (field.type) {
            case "double":
            case "float":
              gen("d%s=o.json&&!isFinite(m%s)?String(m%s):m%s", prop, prop, prop, prop);
              break;
            case "uint64":
              isUnsigned = true;
            // eslint-disable-next-line no-fallthrough
            case "int64":
            case "sint64":
            case "fixed64":
            case "sfixed64":
              gen('if(typeof m%s==="number")', prop)("d%s=o.longs===String?String(m%s):m%s", prop, prop, prop)("else")("d%s=o.longs===String?util.Long.prototype.toString.call(m%s):o.longs===Number?new util.LongBits(m%s.low>>>0,m%s.high>>>0).toNumber(%s):m%s", prop, prop, prop, prop, isUnsigned ? "true" : "", prop);
              break;
            case "bytes":
              gen("d%s=o.bytes===String?util.base64.encode(m%s,0,m%s.length):o.bytes===Array?Array.prototype.slice.call(m%s):m%s", prop, prop, prop, prop, prop);
              break;
            default:
              gen("d%s=m%s", prop, prop);
              break;
          }
        }
        return gen;
      }
      converter.toObject = function toObject(mtype) {
        var fields = mtype.fieldsArray.slice().sort(util2.compareFieldsById);
        if (!fields.length)
          return util2.codegen()("return {}");
        var gen = util2.codegen(["m", "o"], mtype.name + "$toObject")("if(!o)")("o={}")("var d={}");
        var repeatedFields = [], mapFields = [], normalFields = [], i = 0;
        for (; i < fields.length; ++i)
          if (!fields[i].partOf)
            (fields[i].resolve().repeated ? repeatedFields : fields[i].map ? mapFields : normalFields).push(fields[i]);
        if (repeatedFields.length) {
          gen("if(o.arrays||o.defaults){");
          for (i = 0; i < repeatedFields.length; ++i) gen("d%s=[]", util2.safeProp(repeatedFields[i].name));
          gen("}");
        }
        if (mapFields.length) {
          gen("if(o.objects||o.defaults){");
          for (i = 0; i < mapFields.length; ++i) gen("d%s={}", util2.safeProp(mapFields[i].name));
          gen("}");
        }
        if (normalFields.length) {
          gen("if(o.defaults){");
          for (i = 0; i < normalFields.length; ++i) {
            var field = normalFields[i], prop = util2.safeProp(field.name);
            if (field.resolvedType instanceof Enum) gen("d%s=o.enums===String?%j:%j", prop, field.resolvedType.valuesById[field.typeDefault], field.typeDefault);
            else if (field.long) gen("if(util.Long){")("var n=new util.Long(%i,%i,%j)", field.typeDefault.low, field.typeDefault.high, field.typeDefault.unsigned)("d%s=o.longs===String?n.toString():o.longs===Number?n.toNumber():n", prop)("}else")("d%s=o.longs===String?%j:%i", prop, field.typeDefault.toString(), field.typeDefault.toNumber());
            else if (field.bytes) {
              var arrayDefault = "[" + Array.prototype.slice.call(field.typeDefault).join(",") + "]";
              gen("if(o.bytes===String)d%s=%j", prop, String.fromCharCode.apply(String, field.typeDefault))("else{")("d%s=%s", prop, arrayDefault)("if(o.bytes!==Array)d%s=util.newBuffer(d%s)", prop, prop)("}");
            } else gen("d%s=%j", prop, field.typeDefault);
          }
          gen("}");
        }
        var hasKs2 = false;
        for (i = 0; i < fields.length; ++i) {
          var field = fields[i], index = mtype._fieldsArray.indexOf(field), prop = util2.safeProp(field.name);
          if (field.map) {
            if (!hasKs2) {
              hasKs2 = true;
              gen("var ks2");
            }
            gen("if(m%s&&(ks2=Object.keys(m%s)).length){", prop, prop)("d%s={}", prop)("for(var j=0;j<ks2.length;++j){");
            genValuePartial_toObject(
              gen,
              field,
              /* sorted */
              index,
              prop + "[ks2[j]]"
            )("}");
          } else if (field.repeated) {
            gen("if(m%s&&m%s.length){", prop, prop)("d%s=[]", prop)("for(var j=0;j<m%s.length;++j){", prop);
            genValuePartial_toObject(
              gen,
              field,
              /* sorted */
              index,
              prop + "[j]"
            )("}");
          } else {
            gen("if(m%s!=null&&m.hasOwnProperty(%j)){", prop, field.name);
            genValuePartial_toObject(
              gen,
              field,
              /* sorted */
              index,
              prop
            );
            if (field.partOf) gen("if(o.oneofs)")("d%s=%j", util2.safeProp(field.partOf.name), field.name);
          }
          gen("}");
        }
        return gen("return d");
      };
    }
  });

  // node_modules/protobufjs/src/wrappers.js
  var require_wrappers = __commonJS({
    "node_modules/protobufjs/src/wrappers.js"(exports) {
      "use strict";
      var wrappers = exports;
      var Message2 = require_message();
      wrappers[".google.protobuf.Any"] = {
        fromObject: function(object) {
          if (object && object["@type"]) {
            var name = object["@type"].substring(object["@type"].lastIndexOf("/") + 1);
            var type = this.lookup(name);
            if (type) {
              var type_url = object["@type"].charAt(0) === "." ? object["@type"].slice(1) : object["@type"];
              if (type_url.indexOf("/") === -1) {
                type_url = "/" + type_url;
              }
              return this.create({
                type_url,
                value: type.encode(type.fromObject(object)).finish()
              });
            }
          }
          return this.fromObject(object);
        },
        toObject: function(message, options) {
          var googleApi = "type.googleapis.com/";
          var prefix = "";
          var name = "";
          if (options && options.json && message.type_url && message.value) {
            name = message.type_url.substring(message.type_url.lastIndexOf("/") + 1);
            prefix = message.type_url.substring(0, message.type_url.lastIndexOf("/") + 1);
            var type = this.lookup(name);
            if (type)
              message = type.decode(message.value);
          }
          if (!(message instanceof this.ctor) && message instanceof Message2) {
            var object = message.$type.toObject(message, options);
            var messageName = message.$type.fullName[0] === "." ? message.$type.fullName.slice(1) : message.$type.fullName;
            if (prefix === "") {
              prefix = googleApi;
            }
            name = prefix + messageName;
            object["@type"] = name;
            return object;
          }
          return this.toObject(message, options);
        }
      };
    }
  });

  // node_modules/protobufjs/src/type.js
  var require_type = __commonJS({
    "node_modules/protobufjs/src/type.js"(exports, module) {
      "use strict";
      module.exports = Type;
      var Namespace = require_namespace();
      ((Type.prototype = Object.create(Namespace.prototype)).constructor = Type).className = "Type";
      var Enum = require_enum();
      var OneOf = require_oneof();
      var Field = require_field();
      var MapField = require_mapfield();
      var Service = require_service2();
      var Message2 = require_message();
      var Reader = require_reader();
      var Writer = require_writer();
      var util2 = require_util();
      var encoder = require_encoder();
      var decoder = require_decoder();
      var verifier = require_verifier();
      var converter = require_converter();
      var wrappers = require_wrappers();
      function Type(name, options) {
        Namespace.call(this, name, options);
        this.fields = {};
        this.oneofs = void 0;
        this.extensions = void 0;
        this.reserved = void 0;
        this.group = void 0;
        this._fieldsById = null;
        this._fieldsArray = null;
        this._oneofsArray = null;
        this._ctor = null;
      }
      Object.defineProperties(Type.prototype, {
        /**
         * Message fields by id.
         * @name Type#fieldsById
         * @type {Object.<number,Field>}
         * @readonly
         */
        fieldsById: {
          get: function() {
            if (this._fieldsById)
              return this._fieldsById;
            this._fieldsById = {};
            for (var names = Object.keys(this.fields), i = 0; i < names.length; ++i) {
              var field = this.fields[names[i]], id = field.id;
              if (this._fieldsById[id])
                throw Error("duplicate id " + id + " in " + this);
              this._fieldsById[id] = field;
            }
            return this._fieldsById;
          }
        },
        /**
         * Fields of this message as an array for iteration.
         * @name Type#fieldsArray
         * @type {Field[]}
         * @readonly
         */
        fieldsArray: {
          get: function() {
            return this._fieldsArray || (this._fieldsArray = util2.toArray(this.fields));
          }
        },
        /**
         * Oneofs of this message as an array for iteration.
         * @name Type#oneofsArray
         * @type {OneOf[]}
         * @readonly
         */
        oneofsArray: {
          get: function() {
            return this._oneofsArray || (this._oneofsArray = util2.toArray(this.oneofs));
          }
        },
        /**
         * The registered constructor, if any registered, otherwise a generic constructor.
         * Assigning a function replaces the internal constructor. If the function does not extend {@link Message} yet, its prototype will be setup accordingly and static methods will be populated. If it already extends {@link Message}, it will just replace the internal constructor.
         * @name Type#ctor
         * @type {Constructor<{}>}
         */
        ctor: {
          get: function() {
            return this._ctor || (this.ctor = Type.generateConstructor(this)());
          },
          set: function(ctor) {
            var prototype = ctor.prototype;
            if (!(prototype instanceof Message2)) {
              (ctor.prototype = new Message2()).constructor = ctor;
              util2.merge(ctor.prototype, prototype);
            }
            ctor.$type = ctor.prototype.$type = this;
            util2.merge(ctor, Message2, true);
            this._ctor = ctor;
            var i = 0;
            for (; i < /* initializes */
            this.fieldsArray.length; ++i)
              this._fieldsArray[i].resolve();
            var ctorProperties = {};
            for (i = 0; i < /* initializes */
            this.oneofsArray.length; ++i)
              ctorProperties[this._oneofsArray[i].resolve().name] = {
                get: util2.oneOfGetter(this._oneofsArray[i].oneof),
                set: util2.oneOfSetter(this._oneofsArray[i].oneof)
              };
            if (i)
              Object.defineProperties(ctor.prototype, ctorProperties);
          }
        }
      });
      Type.generateConstructor = function generateConstructor(mtype) {
        var gen = util2.codegen(["p"], mtype.name);
        for (var i = 0, field; i < mtype.fieldsArray.length; ++i)
          if ((field = mtype._fieldsArray[i]).map) gen("this%s={}", util2.safeProp(field.name));
          else if (field.repeated) gen("this%s=[]", util2.safeProp(field.name));
        return gen("if(p)for(var ks=Object.keys(p),i=0;i<ks.length;++i)if(p[ks[i]]!=null)")("this[ks[i]]=p[ks[i]]");
      };
      function clearCache(type) {
        type._fieldsById = type._fieldsArray = type._oneofsArray = null;
        delete type.encode;
        delete type.decode;
        delete type.verify;
        return type;
      }
      Type.fromJSON = function fromJSON(name, json) {
        var type = new Type(name, json.options);
        type.extensions = json.extensions;
        type.reserved = json.reserved;
        var names = Object.keys(json.fields), i = 0;
        for (; i < names.length; ++i)
          type.add(
            (typeof json.fields[names[i]].keyType !== "undefined" ? MapField.fromJSON : Field.fromJSON)(names[i], json.fields[names[i]])
          );
        if (json.oneofs)
          for (names = Object.keys(json.oneofs), i = 0; i < names.length; ++i)
            type.add(OneOf.fromJSON(names[i], json.oneofs[names[i]]));
        if (json.nested)
          for (names = Object.keys(json.nested), i = 0; i < names.length; ++i) {
            var nested = json.nested[names[i]];
            type.add(
              // most to least likely
              (nested.id !== void 0 ? Field.fromJSON : nested.fields !== void 0 ? Type.fromJSON : nested.values !== void 0 ? Enum.fromJSON : nested.methods !== void 0 ? Service.fromJSON : Namespace.fromJSON)(names[i], nested)
            );
          }
        if (json.extensions && json.extensions.length)
          type.extensions = json.extensions;
        if (json.reserved && json.reserved.length)
          type.reserved = json.reserved;
        if (json.group)
          type.group = true;
        if (json.comment)
          type.comment = json.comment;
        if (json.edition)
          type._edition = json.edition;
        type._defaultEdition = "proto3";
        return type;
      };
      Type.prototype.toJSON = function toJSON(toJSONOptions) {
        var inherited = Namespace.prototype.toJSON.call(this, toJSONOptions);
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "edition",
          this._editionToJSON(),
          "options",
          inherited && inherited.options || void 0,
          "oneofs",
          Namespace.arrayToJSON(this.oneofsArray, toJSONOptions),
          "fields",
          Namespace.arrayToJSON(this.fieldsArray.filter(function(obj) {
            return !obj.declaringField;
          }), toJSONOptions) || {},
          "extensions",
          this.extensions && this.extensions.length ? this.extensions : void 0,
          "reserved",
          this.reserved && this.reserved.length ? this.reserved : void 0,
          "group",
          this.group || void 0,
          "nested",
          inherited && inherited.nested || void 0,
          "comment",
          keepComments ? this.comment : void 0
        ]);
      };
      Type.prototype.resolveAll = function resolveAll() {
        if (!this._needsRecursiveResolve) return this;
        Namespace.prototype.resolveAll.call(this);
        var oneofs = this.oneofsArray;
        i = 0;
        while (i < oneofs.length)
          oneofs[i++].resolve();
        var fields = this.fieldsArray, i = 0;
        while (i < fields.length)
          fields[i++].resolve();
        return this;
      };
      Type.prototype._resolveFeaturesRecursive = function _resolveFeaturesRecursive(edition) {
        if (!this._needsRecursiveFeatureResolution) return this;
        edition = this._edition || edition;
        Namespace.prototype._resolveFeaturesRecursive.call(this, edition);
        this.oneofsArray.forEach((oneof) => {
          oneof._resolveFeatures(edition);
        });
        this.fieldsArray.forEach((field) => {
          field._resolveFeatures(edition);
        });
        return this;
      };
      Type.prototype.get = function get(name) {
        return this.fields[name] || this.oneofs && this.oneofs[name] || this.nested && this.nested[name] || null;
      };
      Type.prototype.add = function add2(object) {
        if (this.get(object.name))
          throw Error("duplicate name '" + object.name + "' in " + this);
        if (object instanceof Field && object.extend === void 0) {
          if (this._fieldsById ? (
            /* istanbul ignore next */
            this._fieldsById[object.id]
          ) : this.fieldsById[object.id])
            throw Error("duplicate id " + object.id + " in " + this);
          if (this.isReservedId(object.id))
            throw Error("id " + object.id + " is reserved in " + this);
          if (this.isReservedName(object.name))
            throw Error("name '" + object.name + "' is reserved in " + this);
          if (object.parent)
            object.parent.remove(object);
          this.fields[object.name] = object;
          object.message = this;
          object.onAdd(this);
          return clearCache(this);
        }
        if (object instanceof OneOf) {
          if (!this.oneofs)
            this.oneofs = {};
          this.oneofs[object.name] = object;
          object.onAdd(this);
          return clearCache(this);
        }
        return Namespace.prototype.add.call(this, object);
      };
      Type.prototype.remove = function remove(object) {
        if (object instanceof Field && object.extend === void 0) {
          if (!this.fields || this.fields[object.name] !== object)
            throw Error(object + " is not a member of " + this);
          delete this.fields[object.name];
          object.parent = null;
          object.onRemove(this);
          return clearCache(this);
        }
        if (object instanceof OneOf) {
          if (!this.oneofs || this.oneofs[object.name] !== object)
            throw Error(object + " is not a member of " + this);
          delete this.oneofs[object.name];
          object.parent = null;
          object.onRemove(this);
          return clearCache(this);
        }
        return Namespace.prototype.remove.call(this, object);
      };
      Type.prototype.isReservedId = function isReservedId(id) {
        return Namespace.isReservedId(this.reserved, id);
      };
      Type.prototype.isReservedName = function isReservedName(name) {
        return Namespace.isReservedName(this.reserved, name);
      };
      Type.prototype.create = function create(properties) {
        return new this.ctor(properties);
      };
      Type.prototype.setup = function setup() {
        var fullName = this.fullName, types = [];
        for (var i = 0; i < /* initializes */
        this.fieldsArray.length; ++i)
          types.push(this._fieldsArray[i].resolve().resolvedType);
        this.encode = encoder(this)({
          Writer,
          types,
          util: util2
        });
        this.decode = decoder(this)({
          Reader,
          types,
          util: util2
        });
        this.verify = verifier(this)({
          types,
          util: util2
        });
        this.fromObject = converter.fromObject(this)({
          types,
          util: util2
        });
        this.toObject = converter.toObject(this)({
          types,
          util: util2
        });
        var wrapper = wrappers[fullName];
        if (wrapper) {
          var originalThis = Object.create(this);
          originalThis.fromObject = this.fromObject;
          this.fromObject = wrapper.fromObject.bind(originalThis);
          originalThis.toObject = this.toObject;
          this.toObject = wrapper.toObject.bind(originalThis);
        }
        return this;
      };
      Type.prototype.encode = function encode_setup(message, writer) {
        return this.setup().encode(message, writer);
      };
      Type.prototype.encodeDelimited = function encodeDelimited(message, writer) {
        return this.encode(message, writer && writer.len ? writer.fork() : writer).ldelim();
      };
      Type.prototype.decode = function decode_setup(reader, length) {
        return this.setup().decode(reader, length);
      };
      Type.prototype.decodeDelimited = function decodeDelimited(reader) {
        if (!(reader instanceof Reader))
          reader = Reader.create(reader);
        return this.decode(reader, reader.uint32());
      };
      Type.prototype.verify = function verify_setup(message) {
        return this.setup().verify(message);
      };
      Type.prototype.fromObject = function fromObject(object) {
        return this.setup().fromObject(object);
      };
      Type.prototype.toObject = function toObject(message, options) {
        return this.setup().toObject(message, options);
      };
      Type.d = function decorateType(typeName) {
        return function typeDecorator(target) {
          util2.decorateType(target, typeName);
        };
      };
    }
  });

  // node_modules/protobufjs/src/root.js
  var require_root = __commonJS({
    "node_modules/protobufjs/src/root.js"(exports, module) {
      "use strict";
      module.exports = Root2;
      var Namespace = require_namespace();
      ((Root2.prototype = Object.create(Namespace.prototype)).constructor = Root2).className = "Root";
      var Field = require_field();
      var Enum = require_enum();
      var OneOf = require_oneof();
      var util2 = require_util();
      var Type;
      var parse;
      var common;
      function Root2(options) {
        Namespace.call(this, "", options);
        this.deferred = [];
        this.files = [];
        this._edition = "proto2";
        this._fullyQualifiedObjects = {};
      }
      Root2.fromJSON = function fromJSON(json, root2) {
        if (!root2)
          root2 = new Root2();
        if (json.options)
          root2.setOptions(json.options);
        return root2.addJSON(json.nested).resolveAll();
      };
      Root2.prototype.resolvePath = util2.path.resolve;
      Root2.prototype.fetch = util2.fetch;
      function SYNC() {
      }
      Root2.prototype.load = function load(filename, options, callback) {
        if (typeof options === "function") {
          callback = options;
          options = void 0;
        }
        var self2 = this;
        if (!callback) {
          return util2.asPromise(load, self2, filename, options);
        }
        var sync = callback === SYNC;
        function finish(err, root2) {
          if (!callback) {
            return;
          }
          if (sync) {
            throw err;
          }
          if (root2) {
            root2.resolveAll();
          }
          var cb = callback;
          callback = null;
          cb(err, root2);
        }
        function getBundledFileName(filename2) {
          var idx = filename2.lastIndexOf("google/protobuf/");
          if (idx > -1) {
            var altname = filename2.substring(idx);
            if (altname in common) return altname;
          }
          return null;
        }
        function process(filename2, source) {
          try {
            if (util2.isString(source) && source.charAt(0) === "{")
              source = JSON.parse(source);
            if (!util2.isString(source))
              self2.setOptions(source.options).addJSON(source.nested);
            else {
              parse.filename = filename2;
              var parsed = parse(source, self2, options), resolved2, i2 = 0;
              if (parsed.imports) {
                for (; i2 < parsed.imports.length; ++i2)
                  if (resolved2 = getBundledFileName(parsed.imports[i2]) || self2.resolvePath(filename2, parsed.imports[i2]))
                    fetch(resolved2);
              }
              if (parsed.weakImports) {
                for (i2 = 0; i2 < parsed.weakImports.length; ++i2)
                  if (resolved2 = getBundledFileName(parsed.weakImports[i2]) || self2.resolvePath(filename2, parsed.weakImports[i2]))
                    fetch(resolved2, true);
              }
            }
          } catch (err) {
            finish(err);
          }
          if (!sync && !queued) {
            finish(null, self2);
          }
        }
        function fetch(filename2, weak) {
          filename2 = getBundledFileName(filename2) || filename2;
          if (self2.files.indexOf(filename2) > -1) {
            return;
          }
          self2.files.push(filename2);
          if (filename2 in common) {
            if (sync) {
              process(filename2, common[filename2]);
            } else {
              ++queued;
              setTimeout(function() {
                --queued;
                process(filename2, common[filename2]);
              });
            }
            return;
          }
          if (sync) {
            var source;
            try {
              source = util2.fs.readFileSync(filename2).toString("utf8");
            } catch (err) {
              if (!weak)
                finish(err);
              return;
            }
            process(filename2, source);
          } else {
            ++queued;
            self2.fetch(filename2, function(err, source2) {
              --queued;
              if (!callback) {
                return;
              }
              if (err) {
                if (!weak)
                  finish(err);
                else if (!queued)
                  finish(null, self2);
                return;
              }
              process(filename2, source2);
            });
          }
        }
        var queued = 0;
        if (util2.isString(filename)) {
          filename = [filename];
        }
        for (var i = 0, resolved; i < filename.length; ++i)
          if (resolved = self2.resolvePath("", filename[i]))
            fetch(resolved);
        if (sync) {
          self2.resolveAll();
          return self2;
        }
        if (!queued) {
          finish(null, self2);
        }
        return self2;
      };
      Root2.prototype.loadSync = function loadSync(filename, options) {
        if (!util2.isNode)
          throw Error("not supported");
        return this.load(filename, options, SYNC);
      };
      Root2.prototype.resolveAll = function resolveAll() {
        if (!this._needsRecursiveResolve) return this;
        if (this.deferred.length)
          throw Error("unresolvable extensions: " + this.deferred.map(function(field) {
            return "'extend " + field.extend + "' in " + field.parent.fullName;
          }).join(", "));
        return Namespace.prototype.resolveAll.call(this);
      };
      var exposeRe = /^[A-Z]/;
      function tryHandleExtension(root2, field) {
        var extendedType = field.parent.lookup(field.extend);
        if (extendedType) {
          var sisterField = new Field(field.fullName, field.id, field.type, field.rule, void 0, field.options);
          if (extendedType.get(sisterField.name)) {
            return true;
          }
          sisterField.declaringField = field;
          field.extensionField = sisterField;
          extendedType.add(sisterField);
          return true;
        }
        return false;
      }
      Root2.prototype._handleAdd = function _handleAdd(object) {
        if (object instanceof Field) {
          if (
            /* an extension field (implies not part of a oneof) */
            object.extend !== void 0 && /* not already handled */
            !object.extensionField
          ) {
            if (!tryHandleExtension(this, object))
              this.deferred.push(object);
          }
        } else if (object instanceof Enum) {
          if (exposeRe.test(object.name))
            object.parent[object.name] = object.values;
        } else if (!(object instanceof OneOf)) {
          if (object instanceof Type)
            for (var i = 0; i < this.deferred.length; )
              if (tryHandleExtension(this, this.deferred[i]))
                this.deferred.splice(i, 1);
              else
                ++i;
          for (var j = 0; j < /* initializes */
          object.nestedArray.length; ++j)
            this._handleAdd(object._nestedArray[j]);
          if (exposeRe.test(object.name))
            object.parent[object.name] = object;
        }
        if (object instanceof Type || object instanceof Enum || object instanceof Field) {
          this._fullyQualifiedObjects[object.fullName] = object;
        }
      };
      Root2.prototype._handleRemove = function _handleRemove(object) {
        if (object instanceof Field) {
          if (
            /* an extension field */
            object.extend !== void 0
          ) {
            if (
              /* already handled */
              object.extensionField
            ) {
              object.extensionField.parent.remove(object.extensionField);
              object.extensionField = null;
            } else {
              var index = this.deferred.indexOf(object);
              if (index > -1)
                this.deferred.splice(index, 1);
            }
          }
        } else if (object instanceof Enum) {
          if (exposeRe.test(object.name))
            delete object.parent[object.name];
        } else if (object instanceof Namespace) {
          for (var i = 0; i < /* initializes */
          object.nestedArray.length; ++i)
            this._handleRemove(object._nestedArray[i]);
          if (exposeRe.test(object.name))
            delete object.parent[object.name];
        }
        delete this._fullyQualifiedObjects[object.fullName];
      };
      Root2._configure = function(Type_, parse_, common_) {
        Type = Type_;
        parse = parse_;
        common = common_;
      };
    }
  });

  // node_modules/protobufjs/src/util.js
  var require_util = __commonJS({
    "node_modules/protobufjs/src/util.js"(exports, module) {
      "use strict";
      var util2 = module.exports = require_minimal();
      var roots = require_roots();
      var Type;
      var Enum;
      util2.codegen = require_codegen();
      util2.fetch = require_fetch();
      util2.path = require_path();
      util2.fs = util2.inquire("fs");
      util2.toArray = function toArray(object) {
        if (object) {
          var keys = Object.keys(object), array = new Array(keys.length), index = 0;
          while (index < keys.length)
            array[index] = object[keys[index++]];
          return array;
        }
        return [];
      };
      util2.toObject = function toObject(array) {
        var object = {}, index = 0;
        while (index < array.length) {
          var key = array[index++], val = array[index++];
          if (val !== void 0)
            object[key] = val;
        }
        return object;
      };
      var safePropBackslashRe = /\\/g;
      var safePropQuoteRe = /"/g;
      util2.isReserved = function isReserved(name) {
        return /^(?:do|if|in|for|let|new|try|var|case|else|enum|eval|false|null|this|true|void|with|break|catch|class|const|super|throw|while|yield|delete|export|import|public|return|static|switch|typeof|default|extends|finally|package|private|continue|debugger|function|arguments|interface|protected|implements|instanceof)$/.test(name);
      };
      util2.safeProp = function safeProp(prop) {
        if (!/^[$\w_]+$/.test(prop) || util2.isReserved(prop))
          return '["' + prop.replace(safePropBackslashRe, "\\\\").replace(safePropQuoteRe, '\\"') + '"]';
        return "." + prop;
      };
      util2.ucFirst = function ucFirst(str) {
        return str.charAt(0).toUpperCase() + str.substring(1);
      };
      var camelCaseRe = /_([a-z])/g;
      util2.camelCase = function camelCase(str) {
        return str.substring(0, 1) + str.substring(1).replace(camelCaseRe, function($0, $1) {
          return $1.toUpperCase();
        });
      };
      util2.compareFieldsById = function compareFieldsById(a, b) {
        return a.id - b.id;
      };
      util2.decorateType = function decorateType(ctor, typeName) {
        if (ctor.$type) {
          if (typeName && ctor.$type.name !== typeName) {
            util2.decorateRoot.remove(ctor.$type);
            ctor.$type.name = typeName;
            util2.decorateRoot.add(ctor.$type);
          }
          return ctor.$type;
        }
        if (!Type)
          Type = require_type();
        var type = new Type(typeName || ctor.name);
        util2.decorateRoot.add(type);
        type.ctor = ctor;
        Object.defineProperty(ctor, "$type", { value: type, enumerable: false });
        Object.defineProperty(ctor.prototype, "$type", { value: type, enumerable: false });
        return type;
      };
      var decorateEnumIndex = 0;
      util2.decorateEnum = function decorateEnum(object) {
        if (object.$type)
          return object.$type;
        if (!Enum)
          Enum = require_enum();
        var enm = new Enum("Enum" + decorateEnumIndex++, object);
        util2.decorateRoot.add(enm);
        Object.defineProperty(object, "$type", { value: enm, enumerable: false });
        return enm;
      };
      util2.setProperty = function setProperty(dst, path, value, ifNotSet) {
        function setProp(dst2, path2, value2) {
          var part = path2.shift();
          if (part === "__proto__" || part === "prototype") {
            return dst2;
          }
          if (path2.length > 0) {
            dst2[part] = setProp(dst2[part] || {}, path2, value2);
          } else {
            var prevValue = dst2[part];
            if (prevValue && ifNotSet)
              return dst2;
            if (prevValue)
              value2 = [].concat(prevValue).concat(value2);
            dst2[part] = value2;
          }
          return dst2;
        }
        if (typeof dst !== "object")
          throw TypeError("dst must be an object");
        if (!path)
          throw TypeError("path must be specified");
        path = path.split(".");
        return setProp(dst, path, value);
      };
      Object.defineProperty(util2, "decorateRoot", {
        get: function() {
          return roots["decorated"] || (roots["decorated"] = new (require_root())());
        }
      });
    }
  });

  // node_modules/protobufjs/src/types.js
  var require_types = __commonJS({
    "node_modules/protobufjs/src/types.js"(exports) {
      "use strict";
      var types = exports;
      var util2 = require_util();
      var s = [
        "double",
        // 0
        "float",
        // 1
        "int32",
        // 2
        "uint32",
        // 3
        "sint32",
        // 4
        "fixed32",
        // 5
        "sfixed32",
        // 6
        "int64",
        // 7
        "uint64",
        // 8
        "sint64",
        // 9
        "fixed64",
        // 10
        "sfixed64",
        // 11
        "bool",
        // 12
        "string",
        // 13
        "bytes"
        // 14
      ];
      function bake(values, offset) {
        var i = 0, o = {};
        offset |= 0;
        while (i < values.length) o[s[i + offset]] = values[i++];
        return o;
      }
      types.basic = bake([
        /* double   */
        1,
        /* float    */
        5,
        /* int32    */
        0,
        /* uint32   */
        0,
        /* sint32   */
        0,
        /* fixed32  */
        5,
        /* sfixed32 */
        5,
        /* int64    */
        0,
        /* uint64   */
        0,
        /* sint64   */
        0,
        /* fixed64  */
        1,
        /* sfixed64 */
        1,
        /* bool     */
        0,
        /* string   */
        2,
        /* bytes    */
        2
      ]);
      types.defaults = bake([
        /* double   */
        0,
        /* float    */
        0,
        /* int32    */
        0,
        /* uint32   */
        0,
        /* sint32   */
        0,
        /* fixed32  */
        0,
        /* sfixed32 */
        0,
        /* int64    */
        0,
        /* uint64   */
        0,
        /* sint64   */
        0,
        /* fixed64  */
        0,
        /* sfixed64 */
        0,
        /* bool     */
        false,
        /* string   */
        "",
        /* bytes    */
        util2.emptyArray,
        /* message  */
        null
      ]);
      types.long = bake([
        /* int64    */
        0,
        /* uint64   */
        0,
        /* sint64   */
        0,
        /* fixed64  */
        1,
        /* sfixed64 */
        1
      ], 7);
      types.mapKey = bake([
        /* int32    */
        0,
        /* uint32   */
        0,
        /* sint32   */
        0,
        /* fixed32  */
        5,
        /* sfixed32 */
        5,
        /* int64    */
        0,
        /* uint64   */
        0,
        /* sint64   */
        0,
        /* fixed64  */
        1,
        /* sfixed64 */
        1,
        /* bool     */
        0,
        /* string   */
        2
      ], 2);
      types.packed = bake([
        /* double   */
        1,
        /* float    */
        5,
        /* int32    */
        0,
        /* uint32   */
        0,
        /* sint32   */
        0,
        /* fixed32  */
        5,
        /* sfixed32 */
        5,
        /* int64    */
        0,
        /* uint64   */
        0,
        /* sint64   */
        0,
        /* fixed64  */
        1,
        /* sfixed64 */
        1,
        /* bool     */
        0
      ]);
    }
  });

  // node_modules/protobufjs/src/field.js
  var require_field = __commonJS({
    "node_modules/protobufjs/src/field.js"(exports, module) {
      "use strict";
      module.exports = Field;
      var ReflectionObject = require_object();
      ((Field.prototype = Object.create(ReflectionObject.prototype)).constructor = Field).className = "Field";
      var Enum = require_enum();
      var types = require_types();
      var util2 = require_util();
      var Type;
      var ruleRe = /^required|optional|repeated$/;
      Field.fromJSON = function fromJSON(name, json) {
        var field = new Field(name, json.id, json.type, json.rule, json.extend, json.options, json.comment);
        if (json.edition)
          field._edition = json.edition;
        field._defaultEdition = "proto3";
        return field;
      };
      function Field(name, id, type, rule, extend, options, comment) {
        if (util2.isObject(rule)) {
          comment = extend;
          options = rule;
          rule = extend = void 0;
        } else if (util2.isObject(extend)) {
          comment = options;
          options = extend;
          extend = void 0;
        }
        ReflectionObject.call(this, name, options);
        if (!util2.isInteger(id) || id < 0)
          throw TypeError("id must be a non-negative integer");
        if (!util2.isString(type))
          throw TypeError("type must be a string");
        if (rule !== void 0 && !ruleRe.test(rule = rule.toString().toLowerCase()))
          throw TypeError("rule must be a string rule");
        if (extend !== void 0 && !util2.isString(extend))
          throw TypeError("extend must be a string");
        if (rule === "proto3_optional") {
          rule = "optional";
        }
        this.rule = rule && rule !== "optional" ? rule : void 0;
        this.type = type;
        this.id = id;
        this.extend = extend || void 0;
        this.repeated = rule === "repeated";
        this.map = false;
        this.message = null;
        this.partOf = null;
        this.typeDefault = null;
        this.defaultValue = null;
        this.long = util2.Long ? types.long[type] !== void 0 : (
          /* istanbul ignore next */
          false
        );
        this.bytes = type === "bytes";
        this.resolvedType = null;
        this.extensionField = null;
        this.declaringField = null;
        this.comment = comment;
      }
      Object.defineProperty(Field.prototype, "required", {
        get: function() {
          return this._features.field_presence === "LEGACY_REQUIRED";
        }
      });
      Object.defineProperty(Field.prototype, "optional", {
        get: function() {
          return !this.required;
        }
      });
      Object.defineProperty(Field.prototype, "delimited", {
        get: function() {
          return this.resolvedType instanceof Type && this._features.message_encoding === "DELIMITED";
        }
      });
      Object.defineProperty(Field.prototype, "packed", {
        get: function() {
          return this._features.repeated_field_encoding === "PACKED";
        }
      });
      Object.defineProperty(Field.prototype, "hasPresence", {
        get: function() {
          if (this.repeated || this.map) {
            return false;
          }
          return this.partOf || // oneofs
          this.declaringField || this.extensionField || // extensions
          this._features.field_presence !== "IMPLICIT";
        }
      });
      Field.prototype.setOption = function setOption(name, value, ifNotSet) {
        return ReflectionObject.prototype.setOption.call(this, name, value, ifNotSet);
      };
      Field.prototype.toJSON = function toJSON(toJSONOptions) {
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "edition",
          this._editionToJSON(),
          "rule",
          this.rule !== "optional" && this.rule || void 0,
          "type",
          this.type,
          "id",
          this.id,
          "extend",
          this.extend,
          "options",
          this.options,
          "comment",
          keepComments ? this.comment : void 0
        ]);
      };
      Field.prototype.resolve = function resolve() {
        if (this.resolved)
          return this;
        if ((this.typeDefault = types.defaults[this.type]) === void 0) {
          this.resolvedType = (this.declaringField ? this.declaringField.parent : this.parent).lookupTypeOrEnum(this.type);
          if (this.resolvedType instanceof Type)
            this.typeDefault = null;
          else
            this.typeDefault = this.resolvedType.values[Object.keys(this.resolvedType.values)[0]];
        } else if (this.options && this.options.proto3_optional) {
          this.typeDefault = null;
        }
        if (this.options && this.options["default"] != null) {
          this.typeDefault = this.options["default"];
          if (this.resolvedType instanceof Enum && typeof this.typeDefault === "string")
            this.typeDefault = this.resolvedType.values[this.typeDefault];
        }
        if (this.options) {
          if (this.options.packed !== void 0 && this.resolvedType && !(this.resolvedType instanceof Enum))
            delete this.options.packed;
          if (!Object.keys(this.options).length)
            this.options = void 0;
        }
        if (this.long) {
          this.typeDefault = util2.Long.fromNumber(this.typeDefault, this.type.charAt(0) === "u");
          if (Object.freeze)
            Object.freeze(this.typeDefault);
        } else if (this.bytes && typeof this.typeDefault === "string") {
          var buf;
          if (util2.base64.test(this.typeDefault))
            util2.base64.decode(this.typeDefault, buf = util2.newBuffer(util2.base64.length(this.typeDefault)), 0);
          else
            util2.utf8.write(this.typeDefault, buf = util2.newBuffer(util2.utf8.length(this.typeDefault)), 0);
          this.typeDefault = buf;
        }
        if (this.map)
          this.defaultValue = util2.emptyObject;
        else if (this.repeated)
          this.defaultValue = util2.emptyArray;
        else
          this.defaultValue = this.typeDefault;
        if (this.parent instanceof Type)
          this.parent.ctor.prototype[this.name] = this.defaultValue;
        return ReflectionObject.prototype.resolve.call(this);
      };
      Field.prototype._inferLegacyProtoFeatures = function _inferLegacyProtoFeatures(edition) {
        if (edition !== "proto2" && edition !== "proto3") {
          return {};
        }
        var features = {};
        if (this.rule === "required") {
          features.field_presence = "LEGACY_REQUIRED";
        }
        if (this.parent && types.defaults[this.type] === void 0) {
          var type = this.parent.get(this.type.split(".").pop());
          if (type && type instanceof Type && type.group) {
            features.message_encoding = "DELIMITED";
          }
        }
        if (this.getOption("packed") === true) {
          features.repeated_field_encoding = "PACKED";
        } else if (this.getOption("packed") === false) {
          features.repeated_field_encoding = "EXPANDED";
        }
        return features;
      };
      Field.prototype._resolveFeatures = function _resolveFeatures(edition) {
        return ReflectionObject.prototype._resolveFeatures.call(this, this._edition || edition);
      };
      Field.d = function decorateField(fieldId, fieldType, fieldRule, defaultValue) {
        if (typeof fieldType === "function")
          fieldType = util2.decorateType(fieldType).name;
        else if (fieldType && typeof fieldType === "object")
          fieldType = util2.decorateEnum(fieldType).name;
        return function fieldDecorator(prototype, fieldName) {
          util2.decorateType(prototype.constructor).add(new Field(fieldName, fieldId, fieldType, fieldRule, { "default": defaultValue }));
        };
      };
      Field._configure = function configure2(Type_) {
        Type = Type_;
      };
    }
  });

  // node_modules/protobufjs/src/oneof.js
  var require_oneof = __commonJS({
    "node_modules/protobufjs/src/oneof.js"(exports, module) {
      "use strict";
      module.exports = OneOf;
      var ReflectionObject = require_object();
      ((OneOf.prototype = Object.create(ReflectionObject.prototype)).constructor = OneOf).className = "OneOf";
      var Field = require_field();
      var util2 = require_util();
      function OneOf(name, fieldNames, options, comment) {
        if (!Array.isArray(fieldNames)) {
          options = fieldNames;
          fieldNames = void 0;
        }
        ReflectionObject.call(this, name, options);
        if (!(fieldNames === void 0 || Array.isArray(fieldNames)))
          throw TypeError("fieldNames must be an Array");
        this.oneof = fieldNames || [];
        this.fieldsArray = [];
        this.comment = comment;
      }
      OneOf.fromJSON = function fromJSON(name, json) {
        return new OneOf(name, json.oneof, json.options, json.comment);
      };
      OneOf.prototype.toJSON = function toJSON(toJSONOptions) {
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "options",
          this.options,
          "oneof",
          this.oneof,
          "comment",
          keepComments ? this.comment : void 0
        ]);
      };
      function addFieldsToParent(oneof) {
        if (oneof.parent) {
          for (var i = 0; i < oneof.fieldsArray.length; ++i)
            if (!oneof.fieldsArray[i].parent)
              oneof.parent.add(oneof.fieldsArray[i]);
        }
      }
      OneOf.prototype.add = function add2(field) {
        if (!(field instanceof Field))
          throw TypeError("field must be a Field");
        if (field.parent && field.parent !== this.parent)
          field.parent.remove(field);
        this.oneof.push(field.name);
        this.fieldsArray.push(field);
        field.partOf = this;
        addFieldsToParent(this);
        return this;
      };
      OneOf.prototype.remove = function remove(field) {
        if (!(field instanceof Field))
          throw TypeError("field must be a Field");
        var index = this.fieldsArray.indexOf(field);
        if (index < 0)
          throw Error(field + " is not a member of " + this);
        this.fieldsArray.splice(index, 1);
        index = this.oneof.indexOf(field.name);
        if (index > -1)
          this.oneof.splice(index, 1);
        field.partOf = null;
        return this;
      };
      OneOf.prototype.onAdd = function onAdd(parent) {
        ReflectionObject.prototype.onAdd.call(this, parent);
        var self2 = this;
        for (var i = 0; i < this.oneof.length; ++i) {
          var field = parent.get(this.oneof[i]);
          if (field && !field.partOf) {
            field.partOf = self2;
            self2.fieldsArray.push(field);
          }
        }
        addFieldsToParent(this);
      };
      OneOf.prototype.onRemove = function onRemove(parent) {
        for (var i = 0, field; i < this.fieldsArray.length; ++i)
          if ((field = this.fieldsArray[i]).parent)
            field.parent.remove(field);
        ReflectionObject.prototype.onRemove.call(this, parent);
      };
      Object.defineProperty(OneOf.prototype, "isProto3Optional", {
        get: function() {
          if (this.fieldsArray == null || this.fieldsArray.length !== 1) {
            return false;
          }
          var field = this.fieldsArray[0];
          return field.options != null && field.options["proto3_optional"] === true;
        }
      });
      OneOf.d = function decorateOneOf() {
        var fieldNames = new Array(arguments.length), index = 0;
        while (index < arguments.length)
          fieldNames[index] = arguments[index++];
        return function oneOfDecorator(prototype, oneofName) {
          util2.decorateType(prototype.constructor).add(new OneOf(oneofName, fieldNames));
          Object.defineProperty(prototype, oneofName, {
            get: util2.oneOfGetter(fieldNames),
            set: util2.oneOfSetter(fieldNames)
          });
        };
      };
    }
  });

  // node_modules/protobufjs/src/object.js
  var require_object = __commonJS({
    "node_modules/protobufjs/src/object.js"(exports, module) {
      "use strict";
      module.exports = ReflectionObject;
      ReflectionObject.className = "ReflectionObject";
      var OneOf = require_oneof();
      var util2 = require_util();
      var Root2;
      var editions2023Defaults = { enum_type: "OPEN", field_presence: "EXPLICIT", json_format: "ALLOW", message_encoding: "LENGTH_PREFIXED", repeated_field_encoding: "PACKED", utf8_validation: "VERIFY" };
      var proto2Defaults = { enum_type: "CLOSED", field_presence: "EXPLICIT", json_format: "LEGACY_BEST_EFFORT", message_encoding: "LENGTH_PREFIXED", repeated_field_encoding: "EXPANDED", utf8_validation: "NONE" };
      var proto3Defaults = { enum_type: "OPEN", field_presence: "IMPLICIT", json_format: "ALLOW", message_encoding: "LENGTH_PREFIXED", repeated_field_encoding: "PACKED", utf8_validation: "VERIFY" };
      function ReflectionObject(name, options) {
        if (!util2.isString(name))
          throw TypeError("name must be a string");
        if (options && !util2.isObject(options))
          throw TypeError("options must be an object");
        this.options = options;
        this.parsedOptions = null;
        this.name = name;
        this._edition = null;
        this._defaultEdition = "proto2";
        this._features = {};
        this._featuresResolved = false;
        this.parent = null;
        this.resolved = false;
        this.comment = null;
        this.filename = null;
      }
      Object.defineProperties(ReflectionObject.prototype, {
        /**
         * Reference to the root namespace.
         * @name ReflectionObject#root
         * @type {Root}
         * @readonly
         */
        root: {
          get: function() {
            var ptr = this;
            while (ptr.parent !== null)
              ptr = ptr.parent;
            return ptr;
          }
        },
        /**
         * Full name including leading dot.
         * @name ReflectionObject#fullName
         * @type {string}
         * @readonly
         */
        fullName: {
          get: function() {
            var path = [this.name], ptr = this.parent;
            while (ptr) {
              path.unshift(ptr.name);
              ptr = ptr.parent;
            }
            return path.join(".");
          }
        }
      });
      ReflectionObject.prototype.toJSON = /* istanbul ignore next */
      function toJSON() {
        throw Error();
      };
      ReflectionObject.prototype.onAdd = function onAdd(parent) {
        if (this.parent && this.parent !== parent)
          this.parent.remove(this);
        this.parent = parent;
        this.resolved = false;
        var root2 = parent.root;
        if (root2 instanceof Root2)
          root2._handleAdd(this);
      };
      ReflectionObject.prototype.onRemove = function onRemove(parent) {
        var root2 = parent.root;
        if (root2 instanceof Root2)
          root2._handleRemove(this);
        this.parent = null;
        this.resolved = false;
      };
      ReflectionObject.prototype.resolve = function resolve() {
        if (this.resolved)
          return this;
        if (this.root instanceof Root2)
          this.resolved = true;
        return this;
      };
      ReflectionObject.prototype._resolveFeaturesRecursive = function _resolveFeaturesRecursive(edition) {
        return this._resolveFeatures(this._edition || edition);
      };
      ReflectionObject.prototype._resolveFeatures = function _resolveFeatures(edition) {
        if (this._featuresResolved) {
          return;
        }
        var defaults = {};
        if (!edition) {
          throw new Error("Unknown edition for " + this.fullName);
        }
        var protoFeatures = Object.assign(
          this.options ? Object.assign({}, this.options.features) : {},
          this._inferLegacyProtoFeatures(edition)
        );
        if (this._edition) {
          if (edition === "proto2") {
            defaults = Object.assign({}, proto2Defaults);
          } else if (edition === "proto3") {
            defaults = Object.assign({}, proto3Defaults);
          } else if (edition === "2023") {
            defaults = Object.assign({}, editions2023Defaults);
          } else {
            throw new Error("Unknown edition: " + edition);
          }
          this._features = Object.assign(defaults, protoFeatures || {});
          this._featuresResolved = true;
          return;
        }
        if (this.partOf instanceof OneOf) {
          var lexicalParentFeaturesCopy = Object.assign({}, this.partOf._features);
          this._features = Object.assign(lexicalParentFeaturesCopy, protoFeatures || {});
        } else if (this.declaringField) {
        } else if (this.parent) {
          var parentFeaturesCopy = Object.assign({}, this.parent._features);
          this._features = Object.assign(parentFeaturesCopy, protoFeatures || {});
        } else {
          throw new Error("Unable to find a parent for " + this.fullName);
        }
        if (this.extensionField) {
          this.extensionField._features = this._features;
        }
        this._featuresResolved = true;
      };
      ReflectionObject.prototype._inferLegacyProtoFeatures = function _inferLegacyProtoFeatures() {
        return {};
      };
      ReflectionObject.prototype.getOption = function getOption(name) {
        if (this.options)
          return this.options[name];
        return void 0;
      };
      ReflectionObject.prototype.setOption = function setOption(name, value, ifNotSet) {
        if (!this.options)
          this.options = {};
        if (/^features\./.test(name)) {
          util2.setProperty(this.options, name, value, ifNotSet);
        } else if (!ifNotSet || this.options[name] === void 0) {
          if (this.getOption(name) !== value) this.resolved = false;
          this.options[name] = value;
        }
        return this;
      };
      ReflectionObject.prototype.setParsedOption = function setParsedOption(name, value, propName) {
        if (!this.parsedOptions) {
          this.parsedOptions = [];
        }
        var parsedOptions = this.parsedOptions;
        if (propName) {
          var opt = parsedOptions.find(function(opt2) {
            return Object.prototype.hasOwnProperty.call(opt2, name);
          });
          if (opt) {
            var newValue = opt[name];
            util2.setProperty(newValue, propName, value);
          } else {
            opt = {};
            opt[name] = util2.setProperty({}, propName, value);
            parsedOptions.push(opt);
          }
        } else {
          var newOpt = {};
          newOpt[name] = value;
          parsedOptions.push(newOpt);
        }
        return this;
      };
      ReflectionObject.prototype.setOptions = function setOptions(options, ifNotSet) {
        if (options)
          for (var keys = Object.keys(options), i = 0; i < keys.length; ++i)
            this.setOption(keys[i], options[keys[i]], ifNotSet);
        return this;
      };
      ReflectionObject.prototype.toString = function toString2() {
        var className = this.constructor.className, fullName = this.fullName;
        if (fullName.length)
          return className + " " + fullName;
        return className;
      };
      ReflectionObject.prototype._editionToJSON = function _editionToJSON() {
        if (!this._edition || this._edition === "proto3") {
          return void 0;
        }
        return this._edition;
      };
      ReflectionObject._configure = function(Root_) {
        Root2 = Root_;
      };
    }
  });

  // node_modules/protobufjs/src/enum.js
  var require_enum = __commonJS({
    "node_modules/protobufjs/src/enum.js"(exports, module) {
      "use strict";
      module.exports = Enum;
      var ReflectionObject = require_object();
      ((Enum.prototype = Object.create(ReflectionObject.prototype)).constructor = Enum).className = "Enum";
      var Namespace = require_namespace();
      var util2 = require_util();
      function Enum(name, values, options, comment, comments, valuesOptions) {
        ReflectionObject.call(this, name, options);
        if (values && typeof values !== "object")
          throw TypeError("values must be an object");
        this.valuesById = {};
        this.values = Object.create(this.valuesById);
        this.comment = comment;
        this.comments = comments || {};
        this.valuesOptions = valuesOptions;
        this._valuesFeatures = {};
        this.reserved = void 0;
        if (values) {
          for (var keys = Object.keys(values), i = 0; i < keys.length; ++i)
            if (typeof values[keys[i]] === "number")
              this.valuesById[this.values[keys[i]] = values[keys[i]]] = keys[i];
        }
      }
      Enum.prototype._resolveFeatures = function _resolveFeatures(edition) {
        edition = this._edition || edition;
        ReflectionObject.prototype._resolveFeatures.call(this, edition);
        Object.keys(this.values).forEach((key) => {
          var parentFeaturesCopy = Object.assign({}, this._features);
          this._valuesFeatures[key] = Object.assign(parentFeaturesCopy, this.valuesOptions && this.valuesOptions[key] && this.valuesOptions[key].features);
        });
        return this;
      };
      Enum.fromJSON = function fromJSON(name, json) {
        var enm = new Enum(name, json.values, json.options, json.comment, json.comments);
        enm.reserved = json.reserved;
        if (json.edition)
          enm._edition = json.edition;
        enm._defaultEdition = "proto3";
        return enm;
      };
      Enum.prototype.toJSON = function toJSON(toJSONOptions) {
        var keepComments = toJSONOptions ? Boolean(toJSONOptions.keepComments) : false;
        return util2.toObject([
          "edition",
          this._editionToJSON(),
          "options",
          this.options,
          "valuesOptions",
          this.valuesOptions,
          "values",
          this.values,
          "reserved",
          this.reserved && this.reserved.length ? this.reserved : void 0,
          "comment",
          keepComments ? this.comment : void 0,
          "comments",
          keepComments ? this.comments : void 0
        ]);
      };
      Enum.prototype.add = function add2(name, id, comment, options) {
        if (!util2.isString(name))
          throw TypeError("name must be a string");
        if (!util2.isInteger(id))
          throw TypeError("id must be an integer");
        if (this.values[name] !== void 0)
          throw Error("duplicate name '" + name + "' in " + this);
        if (this.isReservedId(id))
          throw Error("id " + id + " is reserved in " + this);
        if (this.isReservedName(name))
          throw Error("name '" + name + "' is reserved in " + this);
        if (this.valuesById[id] !== void 0) {
          if (!(this.options && this.options.allow_alias))
            throw Error("duplicate id " + id + " in " + this);
          this.values[name] = id;
        } else
          this.valuesById[this.values[name] = id] = name;
        if (options) {
          if (this.valuesOptions === void 0)
            this.valuesOptions = {};
          this.valuesOptions[name] = options || null;
        }
        this.comments[name] = comment || null;
        return this;
      };
      Enum.prototype.remove = function remove(name) {
        if (!util2.isString(name))
          throw TypeError("name must be a string");
        var val = this.values[name];
        if (val == null)
          throw Error("name '" + name + "' does not exist in " + this);
        delete this.valuesById[val];
        delete this.values[name];
        delete this.comments[name];
        if (this.valuesOptions)
          delete this.valuesOptions[name];
        return this;
      };
      Enum.prototype.isReservedId = function isReservedId(id) {
        return Namespace.isReservedId(this.reserved, id);
      };
      Enum.prototype.isReservedName = function isReservedName(name) {
        return Namespace.isReservedName(this.reserved, name);
      };
    }
  });

  // node_modules/protobufjs/src/encoder.js
  var require_encoder = __commonJS({
    "node_modules/protobufjs/src/encoder.js"(exports, module) {
      "use strict";
      module.exports = encoder;
      var Enum = require_enum();
      var types = require_types();
      var util2 = require_util();
      function genTypePartial(gen, field, fieldIndex, ref) {
        return field.delimited ? gen("types[%i].encode(%s,w.uint32(%i)).uint32(%i)", fieldIndex, ref, (field.id << 3 | 3) >>> 0, (field.id << 3 | 4) >>> 0) : gen("types[%i].encode(%s,w.uint32(%i).fork()).ldelim()", fieldIndex, ref, (field.id << 3 | 2) >>> 0);
      }
      function encoder(mtype) {
        var gen = util2.codegen(["m", "w"], mtype.name + "$encode")("if(!w)")("w=Writer.create()");
        var i, ref;
        var fields = (
          /* initializes */
          mtype.fieldsArray.slice().sort(util2.compareFieldsById)
        );
        for (var i = 0; i < fields.length; ++i) {
          var field = fields[i].resolve(), index = mtype._fieldsArray.indexOf(field), type = field.resolvedType instanceof Enum ? "int32" : field.type, wireType = types.basic[type];
          ref = "m" + util2.safeProp(field.name);
          if (field.map) {
            gen("if(%s!=null&&Object.hasOwnProperty.call(m,%j)){", ref, field.name)("for(var ks=Object.keys(%s),i=0;i<ks.length;++i){", ref)("w.uint32(%i).fork().uint32(%i).%s(ks[i])", (field.id << 3 | 2) >>> 0, 8 | types.mapKey[field.keyType], field.keyType);
            if (wireType === void 0) gen("types[%i].encode(%s[ks[i]],w.uint32(18).fork()).ldelim().ldelim()", index, ref);
            else gen(".uint32(%i).%s(%s[ks[i]]).ldelim()", 16 | wireType, type, ref);
            gen("}")("}");
          } else if (field.repeated) {
            gen("if(%s!=null&&%s.length){", ref, ref);
            if (field.packed && types.packed[type] !== void 0) {
              gen("w.uint32(%i).fork()", (field.id << 3 | 2) >>> 0)("for(var i=0;i<%s.length;++i)", ref)("w.%s(%s[i])", type, ref)("w.ldelim()");
            } else {
              gen("for(var i=0;i<%s.length;++i)", ref);
              if (wireType === void 0)
                genTypePartial(gen, field, index, ref + "[i]");
              else gen("w.uint32(%i).%s(%s[i])", (field.id << 3 | wireType) >>> 0, type, ref);
            }
            gen("}");
          } else {
            if (field.optional) gen("if(%s!=null&&Object.hasOwnProperty.call(m,%j))", ref, field.name);
            if (wireType === void 0)
              genTypePartial(gen, field, index, ref);
            else gen("w.uint32(%i).%s(%s)", (field.id << 3 | wireType) >>> 0, type, ref);
          }
        }
        return gen("return w");
      }
    }
  });

  // node_modules/protobufjs/src/index-light.js
  var require_index_light = __commonJS({
    "node_modules/protobufjs/src/index-light.js"(exports, module) {
      "use strict";
      var protobuf2 = module.exports = require_index_minimal();
      protobuf2.build = "light";
      function load(filename, root2, callback) {
        if (typeof root2 === "function") {
          callback = root2;
          root2 = new protobuf2.Root();
        } else if (!root2)
          root2 = new protobuf2.Root();
        return root2.load(filename, callback);
      }
      protobuf2.load = load;
      function loadSync(filename, root2) {
        if (!root2)
          root2 = new protobuf2.Root();
        return root2.loadSync(filename);
      }
      protobuf2.loadSync = loadSync;
      protobuf2.encoder = require_encoder();
      protobuf2.decoder = require_decoder();
      protobuf2.verifier = require_verifier();
      protobuf2.converter = require_converter();
      protobuf2.ReflectionObject = require_object();
      protobuf2.Namespace = require_namespace();
      protobuf2.Root = require_root();
      protobuf2.Enum = require_enum();
      protobuf2.Type = require_type();
      protobuf2.Field = require_field();
      protobuf2.OneOf = require_oneof();
      protobuf2.MapField = require_mapfield();
      protobuf2.Service = require_service2();
      protobuf2.Method = require_method();
      protobuf2.Message = require_message();
      protobuf2.wrappers = require_wrappers();
      protobuf2.types = require_types();
      protobuf2.util = require_util();
      protobuf2.ReflectionObject._configure(protobuf2.Root);
      protobuf2.Namespace._configure(protobuf2.Type, protobuf2.Service, protobuf2.Enum);
      protobuf2.Root._configure(protobuf2.Type);
      protobuf2.Field._configure(protobuf2.Type);
    }
  });

  // node_modules/protobufjs/light.js
  var require_light = __commonJS({
    "node_modules/protobufjs/light.js"(exports, module) {
      "use strict";
      module.exports = require_index_light();
    }
  });

  // src/proto.ts
  var protobuf = __toESM(require_light(), 1);

  // node_modules/long/index.js
  var wasm = null;
  try {
    wasm = new WebAssembly.Instance(
      new WebAssembly.Module(
        new Uint8Array([
          // \0asm
          0,
          97,
          115,
          109,
          // version 1
          1,
          0,
          0,
          0,
          // section "type"
          1,
          13,
          2,
          // 0, () => i32
          96,
          0,
          1,
          127,
          // 1, (i32, i32, i32, i32) => i32
          96,
          4,
          127,
          127,
          127,
          127,
          1,
          127,
          // section "function"
          3,
          7,
          6,
          // 0, type 0
          0,
          // 1, type 1
          1,
          // 2, type 1
          1,
          // 3, type 1
          1,
          // 4, type 1
          1,
          // 5, type 1
          1,
          // section "global"
          6,
          6,
          1,
          // 0, "high", mutable i32
          127,
          1,
          65,
          0,
          11,
          // section "export"
          7,
          50,
          6,
          // 0, "mul"
          3,
          109,
          117,
          108,
          0,
          1,
          // 1, "div_s"
          5,
          100,
          105,
          118,
          95,
          115,
          0,
          2,
          // 2, "div_u"
          5,
          100,
          105,
          118,
          95,
          117,
          0,
          3,
          // 3, "rem_s"
          5,
          114,
          101,
          109,
          95,
          115,
          0,
          4,
          // 4, "rem_u"
          5,
          114,
          101,
          109,
          95,
          117,
          0,
          5,
          // 5, "get_high"
          8,
          103,
          101,
          116,
          95,
          104,
          105,
          103,
          104,
          0,
          0,
          // section "code"
          10,
          191,
          1,
          6,
          // 0, "get_high"
          4,
          0,
          35,
          0,
          11,
          // 1, "mul"
          36,
          1,
          1,
          126,
          32,
          0,
          173,
          32,
          1,
          173,
          66,
          32,
          134,
          132,
          32,
          2,
          173,
          32,
          3,
          173,
          66,
          32,
          134,
          132,
          126,
          34,
          4,
          66,
          32,
          135,
          167,
          36,
          0,
          32,
          4,
          167,
          11,
          // 2, "div_s"
          36,
          1,
          1,
          126,
          32,
          0,
          173,
          32,
          1,
          173,
          66,
          32,
          134,
          132,
          32,
          2,
          173,
          32,
          3,
          173,
          66,
          32,
          134,
          132,
          127,
          34,
          4,
          66,
          32,
          135,
          167,
          36,
          0,
          32,
          4,
          167,
          11,
          // 3, "div_u"
          36,
          1,
          1,
          126,
          32,
          0,
          173,
          32,
          1,
          173,
          66,
          32,
          134,
          132,
          32,
          2,
          173,
          32,
          3,
          173,
          66,
          32,
          134,
          132,
          128,
          34,
          4,
          66,
          32,
          135,
          167,
          36,
          0,
          32,
          4,
          167,
          11,
          // 4, "rem_s"
          36,
          1,
          1,
          126,
          32,
          0,
          173,
          32,
          1,
          173,
          66,
          32,
          134,
          132,
          32,
          2,
          173,
          32,
          3,
          173,
          66,
          32,
          134,
          132,
          129,
          34,
          4,
          66,
          32,
          135,
          167,
          36,
          0,
          32,
          4,
          167,
          11,
          // 5, "rem_u"
          36,
          1,
          1,
          126,
          32,
          0,
          173,
          32,
          1,
          173,
          66,
          32,
          134,
          132,
          32,
          2,
          173,
          32,
          3,
          173,
          66,
          32,
          134,
          132,
          130,
          34,
          4,
          66,
          32,
          135,
          167,
          36,
          0,
          32,
          4,
          167,
          11
        ])
      ),
      {}
    ).exports;
  } catch {
  }
  function Long(low, high, unsigned) {
    this.low = low | 0;
    this.high = high | 0;
    this.unsigned = !!unsigned;
  }
  Long.prototype.__isLong__;
  Object.defineProperty(Long.prototype, "__isLong__", { value: true });
  function isLong(obj) {
    return (obj && obj["__isLong__"]) === true;
  }
  function ctz32(value) {
    var c = Math.clz32(value & -value);
    return value ? 31 - c : c;
  }
  Long.isLong = isLong;
  var INT_CACHE = {};
  var UINT_CACHE = {};
  function fromInt(value, unsigned) {
    var obj, cachedObj, cache;
    if (unsigned) {
      value >>>= 0;
      if (cache = 0 <= value && value < 256) {
        cachedObj = UINT_CACHE[value];
        if (cachedObj) return cachedObj;
      }
      obj = fromBits(value, 0, true);
      if (cache) UINT_CACHE[value] = obj;
      return obj;
    } else {
      value |= 0;
      if (cache = -128 <= value && value < 128) {
        cachedObj = INT_CACHE[value];
        if (cachedObj) return cachedObj;
      }
      obj = fromBits(value, value < 0 ? -1 : 0, false);
      if (cache) INT_CACHE[value] = obj;
      return obj;
    }
  }
  Long.fromInt = fromInt;
  function fromNumber(value, unsigned) {
    if (isNaN(value)) return unsigned ? UZERO : ZERO;
    if (unsigned) {
      if (value < 0) return UZERO;
      if (value >= TWO_PWR_64_DBL) return MAX_UNSIGNED_VALUE;
    } else {
      if (value <= -TWO_PWR_63_DBL) return MIN_VALUE;
      if (value + 1 >= TWO_PWR_63_DBL) return MAX_VALUE;
    }
    if (value < 0) return fromNumber(-value, unsigned).neg();
    return fromBits(
      value % TWO_PWR_32_DBL | 0,
      value / TWO_PWR_32_DBL | 0,
      unsigned
    );
  }
  Long.fromNumber = fromNumber;
  function fromBits(lowBits, highBits, unsigned) {
    return new Long(lowBits, highBits, unsigned);
  }
  Long.fromBits = fromBits;
  var pow_dbl = Math.pow;
  function fromString(str, unsigned, radix) {
    if (str.length === 0) throw Error("empty string");
    if (typeof unsigned === "number") {
      radix = unsigned;
      unsigned = false;
    } else {
      unsigned = !!unsigned;
    }
    if (str === "NaN" || str === "Infinity" || str === "+Infinity" || str === "-Infinity")
      return unsigned ? UZERO : ZERO;
    radix = radix || 10;
    if (radix < 2 || 36 < radix) throw RangeError("radix");
    var p;
    if ((p = str.indexOf("-")) > 0) throw Error("interior hyphen");
    else if (p === 0) {
      return fromString(str.substring(1), unsigned, radix).neg();
    }
    var radixToPower = fromNumber(pow_dbl(radix, 8));
    var result = ZERO;
    for (var i = 0; i < str.length; i += 8) {
      var size = Math.min(8, str.length - i), value = parseInt(str.substring(i, i + size), radix);
      if (size < 8) {
        var power = fromNumber(pow_dbl(radix, size));
        result = result.mul(power).add(fromNumber(value));
      } else {
        result = result.mul(radixToPower);
        result = result.add(fromNumber(value));
      }
    }
    result.unsigned = unsigned;
    return result;
  }
  Long.fromString = fromString;
  function fromValue(val, unsigned) {
    if (typeof val === "number") return fromNumber(val, unsigned);
    if (typeof val === "string") return fromString(val, unsigned);
    return fromBits(
      val.low,
      val.high,
      typeof unsigned === "boolean" ? unsigned : val.unsigned
    );
  }
  Long.fromValue = fromValue;
  var TWO_PWR_16_DBL = 1 << 16;
  var TWO_PWR_24_DBL = 1 << 24;
  var TWO_PWR_32_DBL = TWO_PWR_16_DBL * TWO_PWR_16_DBL;
  var TWO_PWR_64_DBL = TWO_PWR_32_DBL * TWO_PWR_32_DBL;
  var TWO_PWR_63_DBL = TWO_PWR_64_DBL / 2;
  var TWO_PWR_24 = fromInt(TWO_PWR_24_DBL);
  var ZERO = fromInt(0);
  Long.ZERO = ZERO;
  var UZERO = fromInt(0, true);
  Long.UZERO = UZERO;
  var ONE = fromInt(1);
  Long.ONE = ONE;
  var UONE = fromInt(1, true);
  Long.UONE = UONE;
  var NEG_ONE = fromInt(-1);
  Long.NEG_ONE = NEG_ONE;
  var MAX_VALUE = fromBits(4294967295 | 0, 2147483647 | 0, false);
  Long.MAX_VALUE = MAX_VALUE;
  var MAX_UNSIGNED_VALUE = fromBits(4294967295 | 0, 4294967295 | 0, true);
  Long.MAX_UNSIGNED_VALUE = MAX_UNSIGNED_VALUE;
  var MIN_VALUE = fromBits(0, 2147483648 | 0, false);
  Long.MIN_VALUE = MIN_VALUE;
  var LongPrototype = Long.prototype;
  LongPrototype.toInt = function toInt() {
    return this.unsigned ? this.low >>> 0 : this.low;
  };
  LongPrototype.toNumber = function toNumber() {
    if (this.unsigned)
      return (this.high >>> 0) * TWO_PWR_32_DBL + (this.low >>> 0);
    return this.high * TWO_PWR_32_DBL + (this.low >>> 0);
  };
  LongPrototype.toString = function toString(radix) {
    radix = radix || 10;
    if (radix < 2 || 36 < radix) throw RangeError("radix");
    if (this.isZero()) return "0";
    if (this.isNegative()) {
      if (this.eq(MIN_VALUE)) {
        var radixLong = fromNumber(radix), div = this.div(radixLong), rem1 = div.mul(radixLong).sub(this);
        return div.toString(radix) + rem1.toInt().toString(radix);
      } else return "-" + this.neg().toString(radix);
    }
    var radixToPower = fromNumber(pow_dbl(radix, 6), this.unsigned), rem = this;
    var result = "";
    while (true) {
      var remDiv = rem.div(radixToPower), intval = rem.sub(remDiv.mul(radixToPower)).toInt() >>> 0, digits = intval.toString(radix);
      rem = remDiv;
      if (rem.isZero()) return digits + result;
      else {
        while (digits.length < 6) digits = "0" + digits;
        result = "" + digits + result;
      }
    }
  };
  LongPrototype.getHighBits = function getHighBits() {
    return this.high;
  };
  LongPrototype.getHighBitsUnsigned = function getHighBitsUnsigned() {
    return this.high >>> 0;
  };
  LongPrototype.getLowBits = function getLowBits() {
    return this.low;
  };
  LongPrototype.getLowBitsUnsigned = function getLowBitsUnsigned() {
    return this.low >>> 0;
  };
  LongPrototype.getNumBitsAbs = function getNumBitsAbs() {
    if (this.isNegative())
      return this.eq(MIN_VALUE) ? 64 : this.neg().getNumBitsAbs();
    var val = this.high != 0 ? this.high : this.low;
    for (var bit = 31; bit > 0; bit--) if ((val & 1 << bit) != 0) break;
    return this.high != 0 ? bit + 33 : bit + 1;
  };
  LongPrototype.isSafeInteger = function isSafeInteger() {
    var top11Bits = this.high >> 21;
    if (!top11Bits) return true;
    if (this.unsigned) return false;
    return top11Bits === -1 && !(this.low === 0 && this.high === -2097152);
  };
  LongPrototype.isZero = function isZero() {
    return this.high === 0 && this.low === 0;
  };
  LongPrototype.eqz = LongPrototype.isZero;
  LongPrototype.isNegative = function isNegative() {
    return !this.unsigned && this.high < 0;
  };
  LongPrototype.isPositive = function isPositive() {
    return this.unsigned || this.high >= 0;
  };
  LongPrototype.isOdd = function isOdd() {
    return (this.low & 1) === 1;
  };
  LongPrototype.isEven = function isEven() {
    return (this.low & 1) === 0;
  };
  LongPrototype.equals = function equals(other) {
    if (!isLong(other)) other = fromValue(other);
    if (this.unsigned !== other.unsigned && this.high >>> 31 === 1 && other.high >>> 31 === 1)
      return false;
    return this.high === other.high && this.low === other.low;
  };
  LongPrototype.eq = LongPrototype.equals;
  LongPrototype.notEquals = function notEquals(other) {
    return !this.eq(
      /* validates */
      other
    );
  };
  LongPrototype.neq = LongPrototype.notEquals;
  LongPrototype.ne = LongPrototype.notEquals;
  LongPrototype.lessThan = function lessThan(other) {
    return this.comp(
      /* validates */
      other
    ) < 0;
  };
  LongPrototype.lt = LongPrototype.lessThan;
  LongPrototype.lessThanOrEqual = function lessThanOrEqual(other) {
    return this.comp(
      /* validates */
      other
    ) <= 0;
  };
  LongPrototype.lte = LongPrototype.lessThanOrEqual;
  LongPrototype.le = LongPrototype.lessThanOrEqual;
  LongPrototype.greaterThan = function greaterThan(other) {
    return this.comp(
      /* validates */
      other
    ) > 0;
  };
  LongPrototype.gt = LongPrototype.greaterThan;
  LongPrototype.greaterThanOrEqual = function greaterThanOrEqual(other) {
    return this.comp(
      /* validates */
      other
    ) >= 0;
  };
  LongPrototype.gte = LongPrototype.greaterThanOrEqual;
  LongPrototype.ge = LongPrototype.greaterThanOrEqual;
  LongPrototype.compare = function compare(other) {
    if (!isLong(other)) other = fromValue(other);
    if (this.eq(other)) return 0;
    var thisNeg = this.isNegative(), otherNeg = other.isNegative();
    if (thisNeg && !otherNeg) return -1;
    if (!thisNeg && otherNeg) return 1;
    if (!this.unsigned) return this.sub(other).isNegative() ? -1 : 1;
    return other.high >>> 0 > this.high >>> 0 || other.high === this.high && other.low >>> 0 > this.low >>> 0 ? -1 : 1;
  };
  LongPrototype.comp = LongPrototype.compare;
  LongPrototype.negate = function negate() {
    if (!this.unsigned && this.eq(MIN_VALUE)) return MIN_VALUE;
    return this.not().add(ONE);
  };
  LongPrototype.neg = LongPrototype.negate;
  LongPrototype.add = function add(addend) {
    if (!isLong(addend)) addend = fromValue(addend);
    var a48 = this.high >>> 16;
    var a32 = this.high & 65535;
    var a16 = this.low >>> 16;
    var a00 = this.low & 65535;
    var b48 = addend.high >>> 16;
    var b32 = addend.high & 65535;
    var b16 = addend.low >>> 16;
    var b00 = addend.low & 65535;
    var c48 = 0, c32 = 0, c16 = 0, c00 = 0;
    c00 += a00 + b00;
    c16 += c00 >>> 16;
    c00 &= 65535;
    c16 += a16 + b16;
    c32 += c16 >>> 16;
    c16 &= 65535;
    c32 += a32 + b32;
    c48 += c32 >>> 16;
    c32 &= 65535;
    c48 += a48 + b48;
    c48 &= 65535;
    return fromBits(c16 << 16 | c00, c48 << 16 | c32, this.unsigned);
  };
  LongPrototype.subtract = function subtract(subtrahend) {
    if (!isLong(subtrahend)) subtrahend = fromValue(subtrahend);
    return this.add(subtrahend.neg());
  };
  LongPrototype.sub = LongPrototype.subtract;
  LongPrototype.multiply = function multiply(multiplier) {
    if (this.isZero()) return this;
    if (!isLong(multiplier)) multiplier = fromValue(multiplier);
    if (wasm) {
      var low = wasm["mul"](this.low, this.high, multiplier.low, multiplier.high);
      return fromBits(low, wasm["get_high"](), this.unsigned);
    }
    if (multiplier.isZero()) return this.unsigned ? UZERO : ZERO;
    if (this.eq(MIN_VALUE)) return multiplier.isOdd() ? MIN_VALUE : ZERO;
    if (multiplier.eq(MIN_VALUE)) return this.isOdd() ? MIN_VALUE : ZERO;
    if (this.isNegative()) {
      if (multiplier.isNegative()) return this.neg().mul(multiplier.neg());
      else return this.neg().mul(multiplier).neg();
    } else if (multiplier.isNegative()) return this.mul(multiplier.neg()).neg();
    if (this.lt(TWO_PWR_24) && multiplier.lt(TWO_PWR_24))
      return fromNumber(this.toNumber() * multiplier.toNumber(), this.unsigned);
    var a48 = this.high >>> 16;
    var a32 = this.high & 65535;
    var a16 = this.low >>> 16;
    var a00 = this.low & 65535;
    var b48 = multiplier.high >>> 16;
    var b32 = multiplier.high & 65535;
    var b16 = multiplier.low >>> 16;
    var b00 = multiplier.low & 65535;
    var c48 = 0, c32 = 0, c16 = 0, c00 = 0;
    c00 += a00 * b00;
    c16 += c00 >>> 16;
    c00 &= 65535;
    c16 += a16 * b00;
    c32 += c16 >>> 16;
    c16 &= 65535;
    c16 += a00 * b16;
    c32 += c16 >>> 16;
    c16 &= 65535;
    c32 += a32 * b00;
    c48 += c32 >>> 16;
    c32 &= 65535;
    c32 += a16 * b16;
    c48 += c32 >>> 16;
    c32 &= 65535;
    c32 += a00 * b32;
    c48 += c32 >>> 16;
    c32 &= 65535;
    c48 += a48 * b00 + a32 * b16 + a16 * b32 + a00 * b48;
    c48 &= 65535;
    return fromBits(c16 << 16 | c00, c48 << 16 | c32, this.unsigned);
  };
  LongPrototype.mul = LongPrototype.multiply;
  LongPrototype.divide = function divide(divisor) {
    if (!isLong(divisor)) divisor = fromValue(divisor);
    if (divisor.isZero()) throw Error("division by zero");
    if (wasm) {
      if (!this.unsigned && this.high === -2147483648 && divisor.low === -1 && divisor.high === -1) {
        return this;
      }
      var low = (this.unsigned ? wasm["div_u"] : wasm["div_s"])(
        this.low,
        this.high,
        divisor.low,
        divisor.high
      );
      return fromBits(low, wasm["get_high"](), this.unsigned);
    }
    if (this.isZero()) return this.unsigned ? UZERO : ZERO;
    var approx, rem, res;
    if (!this.unsigned) {
      if (this.eq(MIN_VALUE)) {
        if (divisor.eq(ONE) || divisor.eq(NEG_ONE))
          return MIN_VALUE;
        else if (divisor.eq(MIN_VALUE)) return ONE;
        else {
          var halfThis = this.shr(1);
          approx = halfThis.div(divisor).shl(1);
          if (approx.eq(ZERO)) {
            return divisor.isNegative() ? ONE : NEG_ONE;
          } else {
            rem = this.sub(divisor.mul(approx));
            res = approx.add(rem.div(divisor));
            return res;
          }
        }
      } else if (divisor.eq(MIN_VALUE)) return this.unsigned ? UZERO : ZERO;
      if (this.isNegative()) {
        if (divisor.isNegative()) return this.neg().div(divisor.neg());
        return this.neg().div(divisor).neg();
      } else if (divisor.isNegative()) return this.div(divisor.neg()).neg();
      res = ZERO;
    } else {
      if (!divisor.unsigned) divisor = divisor.toUnsigned();
      if (divisor.gt(this)) return UZERO;
      if (divisor.gt(this.shru(1)))
        return UONE;
      res = UZERO;
    }
    rem = this;
    while (rem.gte(divisor)) {
      approx = Math.max(1, Math.floor(rem.toNumber() / divisor.toNumber()));
      var log2 = Math.ceil(Math.log(approx) / Math.LN2), delta = log2 <= 48 ? 1 : pow_dbl(2, log2 - 48), approxRes = fromNumber(approx), approxRem = approxRes.mul(divisor);
      while (approxRem.isNegative() || approxRem.gt(rem)) {
        approx -= delta;
        approxRes = fromNumber(approx, this.unsigned);
        approxRem = approxRes.mul(divisor);
      }
      if (approxRes.isZero()) approxRes = ONE;
      res = res.add(approxRes);
      rem = rem.sub(approxRem);
    }
    return res;
  };
  LongPrototype.div = LongPrototype.divide;
  LongPrototype.modulo = function modulo(divisor) {
    if (!isLong(divisor)) divisor = fromValue(divisor);
    if (wasm) {
      var low = (this.unsigned ? wasm["rem_u"] : wasm["rem_s"])(
        this.low,
        this.high,
        divisor.low,
        divisor.high
      );
      return fromBits(low, wasm["get_high"](), this.unsigned);
    }
    return this.sub(this.div(divisor).mul(divisor));
  };
  LongPrototype.mod = LongPrototype.modulo;
  LongPrototype.rem = LongPrototype.modulo;
  LongPrototype.not = function not() {
    return fromBits(~this.low, ~this.high, this.unsigned);
  };
  LongPrototype.countLeadingZeros = function countLeadingZeros() {
    return this.high ? Math.clz32(this.high) : Math.clz32(this.low) + 32;
  };
  LongPrototype.clz = LongPrototype.countLeadingZeros;
  LongPrototype.countTrailingZeros = function countTrailingZeros() {
    return this.low ? ctz32(this.low) : ctz32(this.high) + 32;
  };
  LongPrototype.ctz = LongPrototype.countTrailingZeros;
  LongPrototype.and = function and(other) {
    if (!isLong(other)) other = fromValue(other);
    return fromBits(this.low & other.low, this.high & other.high, this.unsigned);
  };
  LongPrototype.or = function or(other) {
    if (!isLong(other)) other = fromValue(other);
    return fromBits(this.low | other.low, this.high | other.high, this.unsigned);
  };
  LongPrototype.xor = function xor(other) {
    if (!isLong(other)) other = fromValue(other);
    return fromBits(this.low ^ other.low, this.high ^ other.high, this.unsigned);
  };
  LongPrototype.shiftLeft = function shiftLeft(numBits) {
    if (isLong(numBits)) numBits = numBits.toInt();
    if ((numBits &= 63) === 0) return this;
    else if (numBits < 32)
      return fromBits(
        this.low << numBits,
        this.high << numBits | this.low >>> 32 - numBits,
        this.unsigned
      );
    else return fromBits(0, this.low << numBits - 32, this.unsigned);
  };
  LongPrototype.shl = LongPrototype.shiftLeft;
  LongPrototype.shiftRight = function shiftRight(numBits) {
    if (isLong(numBits)) numBits = numBits.toInt();
    if ((numBits &= 63) === 0) return this;
    else if (numBits < 32)
      return fromBits(
        this.low >>> numBits | this.high << 32 - numBits,
        this.high >> numBits,
        this.unsigned
      );
    else
      return fromBits(
        this.high >> numBits - 32,
        this.high >= 0 ? 0 : -1,
        this.unsigned
      );
  };
  LongPrototype.shr = LongPrototype.shiftRight;
  LongPrototype.shiftRightUnsigned = function shiftRightUnsigned(numBits) {
    if (isLong(numBits)) numBits = numBits.toInt();
    if ((numBits &= 63) === 0) return this;
    if (numBits < 32)
      return fromBits(
        this.low >>> numBits | this.high << 32 - numBits,
        this.high >>> numBits,
        this.unsigned
      );
    if (numBits === 32) return fromBits(this.high, 0, this.unsigned);
    return fromBits(this.high >>> numBits - 32, 0, this.unsigned);
  };
  LongPrototype.shru = LongPrototype.shiftRightUnsigned;
  LongPrototype.shr_u = LongPrototype.shiftRightUnsigned;
  LongPrototype.rotateLeft = function rotateLeft(numBits) {
    var b;
    if (isLong(numBits)) numBits = numBits.toInt();
    if ((numBits &= 63) === 0) return this;
    if (numBits === 32) return fromBits(this.high, this.low, this.unsigned);
    if (numBits < 32) {
      b = 32 - numBits;
      return fromBits(
        this.low << numBits | this.high >>> b,
        this.high << numBits | this.low >>> b,
        this.unsigned
      );
    }
    numBits -= 32;
    b = 32 - numBits;
    return fromBits(
      this.high << numBits | this.low >>> b,
      this.low << numBits | this.high >>> b,
      this.unsigned
    );
  };
  LongPrototype.rotl = LongPrototype.rotateLeft;
  LongPrototype.rotateRight = function rotateRight(numBits) {
    var b;
    if (isLong(numBits)) numBits = numBits.toInt();
    if ((numBits &= 63) === 0) return this;
    if (numBits === 32) return fromBits(this.high, this.low, this.unsigned);
    if (numBits < 32) {
      b = 32 - numBits;
      return fromBits(
        this.high << b | this.low >>> numBits,
        this.low << b | this.high >>> numBits,
        this.unsigned
      );
    }
    numBits -= 32;
    b = 32 - numBits;
    return fromBits(
      this.low << b | this.high >>> numBits,
      this.high << b | this.low >>> numBits,
      this.unsigned
    );
  };
  LongPrototype.rotr = LongPrototype.rotateRight;
  LongPrototype.toSigned = function toSigned() {
    if (!this.unsigned) return this;
    return fromBits(this.low, this.high, false);
  };
  LongPrototype.toUnsigned = function toUnsigned() {
    if (this.unsigned) return this;
    return fromBits(this.low, this.high, true);
  };
  LongPrototype.toBytes = function toBytes(le) {
    return le ? this.toBytesLE() : this.toBytesBE();
  };
  LongPrototype.toBytesLE = function toBytesLE() {
    var hi = this.high, lo = this.low;
    return [
      lo & 255,
      lo >>> 8 & 255,
      lo >>> 16 & 255,
      lo >>> 24,
      hi & 255,
      hi >>> 8 & 255,
      hi >>> 16 & 255,
      hi >>> 24
    ];
  };
  LongPrototype.toBytesBE = function toBytesBE() {
    var hi = this.high, lo = this.low;
    return [
      hi >>> 24,
      hi >>> 16 & 255,
      hi >>> 8 & 255,
      hi & 255,
      lo >>> 24,
      lo >>> 16 & 255,
      lo >>> 8 & 255,
      lo & 255
    ];
  };
  Long.fromBytes = function fromBytes(bytes, unsigned, le) {
    return le ? Long.fromBytesLE(bytes, unsigned) : Long.fromBytesBE(bytes, unsigned);
  };
  Long.fromBytesLE = function fromBytesLE(bytes, unsigned) {
    return new Long(
      bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24,
      bytes[4] | bytes[5] << 8 | bytes[6] << 16 | bytes[7] << 24,
      unsigned
    );
  };
  Long.fromBytesBE = function fromBytesBE(bytes, unsigned) {
    return new Long(
      bytes[4] << 24 | bytes[5] << 16 | bytes[6] << 8 | bytes[7],
      bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3],
      unsigned
    );
  };
  if (typeof BigInt === "function") {
    Long.fromBigInt = function fromBigInt(value, unsigned) {
      var lowBits = Number(BigInt.asIntN(32, value));
      var highBits = Number(BigInt.asIntN(32, value >> BigInt(32)));
      return fromBits(lowBits, highBits, unsigned);
    };
    Long.fromValue = function fromValueWithBigInt(value, unsigned) {
      if (typeof value === "bigint") return Long.fromBigInt(value, unsigned);
      return fromValue(value, unsigned);
    };
    LongPrototype.toBigInt = function toBigInt() {
      var lowBigInt = BigInt(this.low >>> 0);
      var highBigInt = BigInt(this.unsigned ? this.high >>> 0 : this.high);
      return highBigInt << BigInt(32) | lowBigInt;
    };
  }
  var long_default = Long;

  // src/proto.ts
  protobuf.util.Long = long_default;
  protobuf.configure();
  var root = null;
  function initProto(descriptorJson) {
    root = protobuf.Root.fromJSON(JSON.parse(descriptorJson));
  }
  function lookup(name) {
    if (!root) {
      throw new Error("Proto descriptor not initialised (missing proto-descriptor.json)");
    }
    return root.lookupType(name);
  }
  var Proto = {
    // bytes -> message instance (64-bit fields are Long objects)
    decode(name, bytes) {
      return lookup(name).decode(bytes);
    },
    // plain object or message instance -> wire bytes
    encode(name, obj) {
      const type = lookup(name);
      const message = obj instanceof protobuf.Message ? obj : type.fromObject(obj);
      return type.encode(message).finish();
    },
    // message/bytes -> plain JSON-safe object; uint64 becomes string (boundary rule)
    toObject(name, message) {
      return lookup(name).toObject(message, {
        longs: String,
        bytes: String,
        // base64 at the JSON boundary
        enums: Number,
        defaults: false
      });
    },
    decodeToObject(name, bytes) {
      return this.toObject(name, this.decode(name, bytes));
    }
  };

  // src/base64.ts
  var ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
  var REVERSE = {};
  for (let i = 0; i < ALPHABET.length; i++) {
    REVERSE[ALPHABET[i]] = i;
  }
  function b64decode(input) {
    const clean = input.replace(/[=\s]+$/g, "");
    const out = new Uint8Array(Math.floor(clean.length * 3 / 4));
    let o = 0;
    for (let i = 0; i + 1 < clean.length; i += 4) {
      const a = REVERSE[clean[i]];
      const b = REVERSE[clean[i + 1]];
      const c = i + 2 < clean.length ? REVERSE[clean[i + 2]] : -1;
      const d = i + 3 < clean.length ? REVERSE[clean[i + 3]] : -1;
      out[o++] = a << 2 | b >> 4;
      if (c >= 0) out[o++] = (b & 15) << 4 | c >> 2;
      if (d >= 0) out[o++] = (c & 3) << 6 | d;
    }
    return out.subarray(0, o);
  }
  function b64encode(bytes) {
    let out = "";
    for (let i = 0; i < bytes.length; i += 3) {
      const a = bytes[i];
      const b = i + 1 < bytes.length ? bytes[i + 1] : -1;
      const c = i + 2 < bytes.length ? bytes[i + 2] : -1;
      out += ALPHABET[a >> 2];
      out += ALPHABET[(a & 3) << 4 | (b >= 0 ? b >> 4 : 0)];
      out += b >= 0 ? ALPHABET[(b & 15) << 2 | (c >= 0 ? c >> 6 : 0)] : "=";
      out += c >= 0 ? ALPHABET[c & 63] : "=";
    }
    return out;
  }

  // src/index.ts
  var emissions = [];
  var handledFlag = true;
  var notHandledReason = null;
  var gc = {
    reply(responseMsgId, bytes) {
      emissions.push({ kind: "reply", msgId: responseMsgId, payloadBase64: b64encode(bytes) });
    },
    proto(msgId, bytes) {
      emissions.push({ kind: "proto", msgId, payloadBase64: b64encode(bytes) });
    },
    queueReplyTo(steamId, msgId, bytes, targetJobId) {
      emissions.push({
        kind: "queueReplyTo",
        msgId,
        payloadBase64: b64encode(bytes),
        targetSteamId: steamId,
        targetJobId
      });
    },
    queueToServer(serverSteamId, msgId, bytes) {
      emissions.push({
        kind: "queueToServer",
        msgId,
        payloadBase64: b64encode(bytes),
        targetSteamId: serverSteamId
      });
    },
    notHandled(reason) {
      handledFlag = false;
      notHandledReason = reason;
    }
  };
  var handlers = /* @__PURE__ */ new Map();
  function handle(env) {
    const handler = handlers.get(env.messageType);
    if (!handler) {
      gc.notHandled("noop: no JS handler registered for msg " + env.messageType);
      return;
    }
    handler(env);
  }
  function tick(_ctx) {
  }
  var g = globalThis;
  g.__dispatch = function(envelopeJson) {
    const raw = JSON.parse(envelopeJson);
    emissions = [];
    handledFlag = true;
    notHandledReason = null;
    const env = {
      appId: raw.appId,
      steamId: raw.steamId ?? "0",
      accountId: raw.accountId ?? 0,
      personaName: raw.personaName ?? "",
      clientIp: raw.clientIp ?? "",
      messageType: raw.messageType,
      sourceJobId: raw.sourceJobId ?? null,
      targetJobId: raw.targetJobId ?? null,
      isGameServer: raw.isGameServer === true,
      clientVersion: raw.clientVersion ?? null,
      payload: b64decode(raw.payloadBase64 ?? "")
    };
    try {
      handle(env);
    } catch (error) {
      handledFlag = false;
      notHandledReason = "exception: " + String(error);
      emissions = [];
    }
    const result = {
      handled: handledFlag,
      reason: notHandledReason,
      messages: handledFlag ? emissions : []
    };
    return JSON.stringify(result);
  };
  g.__tick = function(ctxJson) {
    try {
      tick(JSON.parse(ctxJson));
      return "{}";
    } catch (error) {
      return JSON.stringify({ error: String(error) });
    }
  };
  g.__proto_decode_json = function(typeName, payloadBase64) {
    return JSON.stringify(Proto.decodeToObject(typeName, b64decode(payloadBase64)));
  };
  g.__proto_encode_b64 = function(typeName, objectJson) {
    return b64encode(Proto.encode(typeName, JSON.parse(objectJson)));
  };
  g.__proto_roundtrip_bench = function(typeName, payloadBase64, iterations) {
    const bytes = b64decode(payloadBase64);
    let lastLength = 0;
    for (let i = 0; i < iterations; i++) {
      const message = Proto.decode(typeName, bytes);
      lastLength = Proto.encode(typeName, message).length;
    }
    return JSON.stringify({ iterations, inputLength: bytes.length, outputLength: lastLength });
  };
  g.__gc_bundle_version = "0.1.0";
  if (typeof g.__PROTO_DESCRIPTOR__ === "string") {
    initProto(g.__PROTO_DESCRIPTOR__);
    if (typeof log === "function") {
      log("gc.js bundle " + g.__gc_bundle_version + " loaded; proto descriptor initialised");
    }
  }
})();
/*! Bundled license information:

long/index.js:
  (**
   * @license
   * Copyright 2009 The Closure Library Authors
   * Copyright 2020 Daniel Wirtz / The long.js Authors.
   *
   * Licensed under the Apache License, Version 2.0 (the "License");
   * you may not use this file except in compliance with the License.
   * You may obtain a copy of the License at
   *
   *     http://www.apache.org/licenses/LICENSE-2.0
   *
   * Unless required by applicable law or agreed to in writing, software
   * distributed under the License is distributed on an "AS IS" BASIS,
   * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   * See the License for the specific language governing permissions and
   * limitations under the License.
   *
   * SPDX-License-Identifier: Apache-2.0
   *)
*/
