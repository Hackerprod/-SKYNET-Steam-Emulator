OPTION CASEMAP:NONE

EXTERN ResolveSteamApiExport:PROC
PUBLIC ProxyJump

.code

ProxyJump PROC
    sub rsp, 0C8h
    mov [rsp+20h], rcx
    mov [rsp+28h], rdx
    mov [rsp+30h], r8
    mov [rsp+38h], r9
    movdqu xmmword ptr [rsp+40h], xmm0
    movdqu xmmword ptr [rsp+50h], xmm1
    movdqu xmmword ptr [rsp+60h], xmm2
    movdqu xmmword ptr [rsp+70h], xmm3
    movdqu xmmword ptr [rsp+80h], xmm4
    movdqu xmmword ptr [rsp+90h], xmm5

    mov ecx, r10d
    call ResolveSteamApiExport
    mov r11, rax

    movdqu xmm0, xmmword ptr [rsp+40h]
    movdqu xmm1, xmmword ptr [rsp+50h]
    movdqu xmm2, xmmword ptr [rsp+60h]
    movdqu xmm3, xmmword ptr [rsp+70h]
    movdqu xmm4, xmmword ptr [rsp+80h]
    movdqu xmm5, xmmword ptr [rsp+90h]
    mov rcx, [rsp+20h]
    mov rdx, [rsp+28h]
    mov r8, [rsp+30h]
    mov r9, [rsp+38h]
    add rsp, 0C8h
    jmp r11
ProxyJump ENDP

END
