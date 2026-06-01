using System;

namespace func.brainfuck
{
    public class BrainfuckBasicCommands
    {
        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            vm.RegisterCommand('.', machine => { write((char)machine.Memory[machine.MemoryPointer]); });
            vm.RegisterCommand('+', machine => { unchecked { machine.Memory[machine.MemoryPointer]++; } });
            vm.RegisterCommand('-', machine => { unchecked { machine.Memory[machine.MemoryPointer]--; } });
            vm.RegisterCommand(',', machine => { machine.Memory[machine.MemoryPointer] = (byte)read(); });
            vm.RegisterCommand('<', machine => MovePointer(machine, -1));
            vm.RegisterCommand('>', machine => MovePointer(machine, +1));
            RegisterAsciiRange(vm, '0', '9');
            RegisterAsciiRange(vm, 'A', 'Z');
            RegisterAsciiRange(vm, 'a', 'z');
        }

        private static void MovePointer(IVirtualMachine machine, int delta) =>
            machine.MemoryPointer = (machine.MemoryPointer + delta + machine.Memory.Length) % machine.Memory.Length;

        private static void RegisterAsciiRange(IVirtualMachine vm, char start, char end)
        {
            for (var ch = start; ch <= end; ch++)
            {
                var capturedChar = ch;
                vm.RegisterCommand(
                    capturedChar,
                    machine => machine.Memory[machine.MemoryPointer] = (byte)capturedChar);
            }
        }
    }
}
