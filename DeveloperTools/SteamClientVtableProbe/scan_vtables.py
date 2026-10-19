import pefile,struct,sys,re
p=pefile.PE(sys.argv[1],fast_load=True)
base=p.OPTIONAL_HEADER.ImageBase
secs=[(s.Name.rstrip(b'\0').decode(),s.VirtualAddress,s.Misc_VirtualSize,s) for s in p.sections]
data=open(sys.argv[1],'rb').read()
def rva2off(r):
    for n,v,sz,s in secs:
        if v<=r<v+max(sz,s.SizeOfRawData): return s.PointerToRawData+(r-v)
def sec(r):
    for n,v,sz,s in secs:
        if v<=r<v+sz: return n
# type descriptors: ".?AV<name>@@"
pat=re.compile(rb'\.\?AV([A-Za-z0-9_]+)@@\0')
want=set(a.encode() for a in sys.argv[2:])
tds={}
for m in pat.finditer(data):
    nm=m.group(1)
    if nm in want: tds.setdefault(nm.decode(),[]).append(m.start()-16)  # TypeDescriptor starts 16 bytes (x64) before name
# map file offsets to rva
def off2rva(o):
    for n,v,sz,s in secs:
        if s.PointerToRawData<=o<s.PointerToRawData+s.SizeOfRawData: return v+(o-s.PointerToRawData)
res={}
for nm,offs in tds.items():
    for o in offs:
        td_rva=off2rva(o)
        # COL (x64): sig=1, off, cdOff, typeDesc rva(4)  -> find 'td_rva' dwords where struct looks like COL
        for m in re.finditer(re.escape(struct.pack('<I',td_rva)),data):
            c=m.start()-12
            if c<0: continue
            sig,off,cd=struct.unpack('<III',data[c:c+12])
            if sig!=1 or off!=0: continue
            col_rva=off2rva(c)
            col_va=base+col_rva
            # vtable: qword == col_va ; vtable at +8
            for mm in re.finditer(re.escape(struct.pack('<Q',col_va)),data):
                vt_off=mm.start()+8
                ents=[]
                i=vt_off
                while True:
                    q=struct.unpack('<Q',data[i:i+8])[0]
                    r=q-base
                    if sec(r)!='.text': break
                    ents.append(r); i+=8
                    if len(ents)>400: break
                res.setdefault(nm,[]).append((off2rva(vt_off),ents))
for nm,l in res.items():
    for vt,ents in l:
        print(nm,"vtable_rva=%x slots=%d"%(vt,len(ents)))
        if len(sys.argv)<0: pass
import json,os
json.dump({k:[[v,[hex(e) for e in es]] for v,es in l] for k,l in res.items()},open(os.path.join(os.path.dirname(sys.argv[1]),'..','vt','out.json'),'w'))
