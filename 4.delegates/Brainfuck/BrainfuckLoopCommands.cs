using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    public class BrainfuckLoopCommands
    {
        public static void RegisterTo(IVirtualMachine vm)
        {
            var jumps = BuildJumpTable(vm.Instructions);

            vm.RegisterCommand('[', b =>
            {
                if (b.Memory[b.MemoryPointer] == 0 && jumps.TryGetValue(b.InstructionPointer, out var target))
                    b.InstructionPointer = target;
            });

            vm.RegisterCommand(']', b =>
            {
                if (b.Memory[b.MemoryPointer] != 0 && jumps.TryGetValue(b.InstructionPointer, out var target))
                    b.InstructionPointer = target;
            });
        }

        private static Dictionary<int, int> BuildJumpTable(string instructions)
        {
            var stack = new Stack<int>();
            var jumps = new Dictionary<int, int>(instructions.Length);

            for (var i = 0; i < instructions.Length; i++)
            {
                var c = instructions[i];
                if (c == '[')
                {
                    stack.Push(i);
                }
                else if (c == ']')
                {
                    var open = stack.Pop();
                    jumps[open] = i;
                    jumps[i] = open;
                }
            }
            return jumps;
        }
    }
}
