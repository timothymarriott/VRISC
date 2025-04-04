# VRISC
VRISC (Vortex Reduced Instruction Set Computer) is a 32 bit theortical computer.\
This repository contains an emulator, assembler and compiler.

## Instruction Set
### BGE
`BGE 0`\
If the carry flag is 1 the program counter is set to the first opcode and the current program counter + 5 (+5 is due to the instruction size) is pushed to the call stack.

### LDI
`LDI 0`\
Sets the register to the first opcode.

### CMP
`CMP`\
Sets the carry flag if the top of the stack is equal to the register.

### ADD
`ADD`\
Adds the register, top of the stack, and Carry flag.

### PUSH
`PUSH`\
Pushes the register to the top of the stack.

### POP
`POP`\
Pops the top of the stack into the register.

### LDR
`LDR`\
Sets the register to the value in memory with the address that matches the top of the stack.

### STR
`STR`\
Sets the memory address at the register to the top of the stack.

### CLC
`CLC`\
Sets the carry flag to 0.

### SEC
`SEC`\
Sets the carry flag to 1.

### RET
`RET`\
If the carry flag is 1 the program counter goes to the value at the top of the call stack.

### HLT
`HLT`\
Exits the execution.

### GRT
`GRT`\
Checks if the register is larger than the top of the stack.

### SUB
`SUB`\
Subtracts the register from the top of the stack and stores it into the register.

### MUL
`MUL`\
Sets the register to the top of the stack multiplied by the register and the carry flag.

### DIV
`DIV`\
Sets the register to the top of the stack divided by the register.

### SYS
`SYS`\
Executes the syscall with the id stored in the register. For syscall ids see the SysCalls section.

### LES
`LES`\
Checks if the register is less than the top of the stack.

### CPOP
`CPOP`\
Pops a value from the call stack.

### LSHFT
`LSHFT`\
Sets the register to the result of shifting to top of the stack register bits to the left.

### RSHFT
`RSHFT`\
Sets the register to the result of shifting to top of the stack register bits to the right.

### IRET
`IRET`\
Return from an interrupt.

## Memory
Memory is addressed by a 32 bit unsinged integeter.\
```
0x00000000 - 0x00FFFFFF - RAM
0x01000000 - 0x01FFFFFF - VRAM
0x02000000 - 0x02FFFFFF - System Page
0x03000000 - 0x03FFFFFF - Stack
0x04000000 - 0x04FFFFFF - Program ROM
```

### System page

| Offset | Size | Name            | Type  | Description                  |     |
| ------ | ---- | --------------- | ----- | ---------------------------- | --- |
| 0x00   | 0x04 | Program Counter | Int32 | The current program counter. |     |
| 0x04   | 0x04 | Carry Flag      | Int32 | The Current carry flag.      |     |
| 0x08   | 0x04 | Stack Pointer   | Int32 | The item count in the stack. |     |


## Syscalls

### 0 : EXIT
Exits the system with the error code at the top of the stack.

### 1 : PRINT
Prints the string in memory at the address stored on the top of the stack.

### 2 : ASSERT
Logs the value at the top of the stack. This value also gets added to a list to be used in a test suite.

### 3 : BLIT
Request to blit the vram to the screen.

### 4 : INIT
Inform the system that the program has initialized.

### 5 : MODE
Toggles the bit mode.

### 6 : READ
Reads the contents of a file and puts it into system memory.

| Stack Index | Type   | Name         | Description                               |
|-------------| ------ | ------------ | ----------------------------------------- |
| 0           | String | Path         | The string path of the file               |
| 1           | Int    | Offset       | The offset into the page to load into     |
| 2           | Int    | Chunk Offset | The offset into the file to load from     |
| 3           | Int    | Chunk Size   | The size of the chunk to load into memory |

### 7 : DIS
Sets the display mode to the value at the top of the stack.\
Then sets the width and height of the frambuffer to be the next two items on the stack.

#### Modes
0 - Text Output (Default)\
1 - Black and white\
2 - Palette\
3 - Bitmap
