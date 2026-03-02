using System;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class ListModel<TItem>
{
    public List<TItem> Items { get; }
    private readonly LimitedSizeStack<ItemCommand> undoStack;

    private class ItemCommand(ItemCommand.CommandType type, TItem item, int index)
    {
        public enum CommandType
        {
            Add,
            Remove
        }

        public CommandType Type { get; } = type;
        public TItem Item { get; } = item;
        public int Index { get; } = index;
    }

    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
        undoStack = new LimitedSizeStack<ItemCommand>(undoLimit);
    }

    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items;
        undoStack = new LimitedSizeStack<ItemCommand>(undoLimit);
    }

    public void AddItem(TItem item)
    {
        Items.Add(item);
        undoStack.Push(new ItemCommand(ItemCommand.CommandType.Add, item, Items.Count - 1));
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        var removedItem = Items[index];
        Items.RemoveAt(index);
        undoStack.Push(new ItemCommand(ItemCommand.CommandType.Remove, removedItem, index));
    }

    public bool CanUndo()
    {
        return undoStack.Count > 0;
    }

    public void Undo()
    {
        if (!CanUndo())
        {
            throw new InvalidOperationException("No actions to undo.");
        }

        var command = undoStack.Pop();
        switch (command.Type)
        {
            case ItemCommand.CommandType.Add:
                Items.RemoveAt(command.Index);
                break;
            case ItemCommand.CommandType.Remove:
                Items.Insert(command.Index, command.Item);
                break;
        }
    }
}
