using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    public class VirtualMachine : IVirtualMachine
    {
        public string Instructions { get; }
        public int InstructionPointer { get; set; }
        public byte[] Memory { get; }
        public int MemoryPointer { get; set; }
        private readonly Dictionary<char, Action<IVirtualMachine>> commands = new();


        public VirtualMachine(string program, int memorySize)
        {
            Memory = new byte[memorySize];
            Instructions = program;
            MemoryPointer = 0;
            InstructionPointer = 0;
        }

        public void RegisterCommand(char symbol, Action<IVirtualMachine> execute)
        {
            commands.Add(symbol, execute);
        }

        public void Run()
        {
            while (InstructionPointer < Instructions.Length)
            {
                if (commands.TryGetValue(Instructions[InstructionPointer], out var command))
                {
                    command(this);
                }
                InstructionPointer++;
            }
        }
    }
}
