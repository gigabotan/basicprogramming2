using System;
using System.Collections.Generic;

namespace Clones;

public class CloneVersionSystem : ICloneVersionSystem
{
    private class LearnNode
    {
        public string Program { get; }
        public LearnNode Previous { get; }

        public LearnNode(string program, LearnNode previous)
        {
            Program = program;
            Previous = previous;
        }
    }

    private readonly List<LearnNode> dataTree = [new("basic", null)];
    private readonly List<LearnNode> undoTree = [null];

    public string Execute(string query)
    {
        var parts = query.Split(' ');
        var cmd = parts[0];
        var id = int.Parse(parts[1]) - 1;
        if (cmd == "learn") Learn(id, parts[2]);
        if (cmd == "rollback") Rollback(id);
        if (cmd == "relearn") Relearn(id);
        if (cmd == "clone") Clone(id);
        return cmd == "check" ? dataTree[id].Program : null;
    }

    private void Learn(int id, string program)
    {
        dataTree[id] = new LearnNode(program, dataTree[id]);
        undoTree[id] = null;
    }

    private void Rollback(int id)
    {
        undoTree[id] = new LearnNode(dataTree[id].Program, undoTree[id]);
        dataTree[id] = dataTree[id].Previous;
    }

    private void Relearn(int id)
    {
        dataTree[id] = new LearnNode(undoTree[id].Program, dataTree[id]);
        undoTree[id] = undoTree[id].Previous;
    }

    private void Clone(int id)
    {
        dataTree.Add(dataTree[id]);
        undoTree.Add(undoTree[id]);
    }
}
