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

## Syscalls

### 0 : EXIT
Exits the system with the error code at the top of the stack.

### 1 : PRINT
Prints the string in memory at the address stored on the top of the stack.

### 2 : ASSERT
Logs the value at the top of the stack. This value also gets added to a list to be used in a test suite.

### 3 : BLIT
Request to blit the vram to the screen.

### 5 : INIT
Inform the system that the program has initialized.

### 6 : READ
Reads the contents of a file and puts it into system memory.


| Stack Index | Type   | Name         | Description                               |
| ----------- | ------ | ------------ | ----------------------------------------- |
| 0           | String | Path         | The string path of the file               |
| 1           | Int    | Page         | The target page to load to                |
| 2           | Int    | Offset       | The offset into the page to load into     |
| 3           | Int    | Chunk Offset | The offset into the file to load from     |
| 4           | Int    | Chunk Size   | The size of the chunk to load into memory |
