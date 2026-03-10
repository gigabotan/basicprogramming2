using System;

namespace func.brainfuck
{
    public class BrainfuckBasicCommands
    {
        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            vm.RegisterCommand('.', b => { write((char)b.Memory[b.MemoryPointer]); });
            vm.RegisterCommand('+', b => { unchecked { b.Memory[b.MemoryPointer]++; } });
            vm.RegisterCommand('-', b => { unchecked { b.Memory[b.MemoryPointer]--; } });
            vm.RegisterCommand(',', b => { b.Memory[b.MemoryPointer] = (byte)read(); });
            vm.RegisterCommand('<', b => MovePointer(b, -1));
            vm.RegisterCommand('>', b => MovePointer(b, +1));
            RegisterAsciiRange(vm, '0', '9');
            RegisterAsciiRange(vm, 'A', 'Z');
            RegisterAsciiRange(vm, 'a', 'z');
        }

        private static void MovePointer(IVirtualMachine b, int delta) =>
            b.MemoryPointer = (b.MemoryPointer + delta + b.Memory.Length) % b.Memory.Length;

        private static void RegisterAsciiRange(IVirtualMachine vm, char start, char end)
        {
            for (var c = start; c <= end; c++)
            {
                var cc = c;
                vm.RegisterCommand(cc, b => { b.Memory[b.MemoryPointer] = (byte)cc; });
            }
        }
    }
}
