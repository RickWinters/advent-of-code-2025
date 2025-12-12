namespace AoC.Helpers.GraphClasses;

public class GraphNode<T> where T : notnull
{
    public T Value { get; }
    public List<GraphEdge<T>> Edges { get; } = new();

    public GraphNode(T value)
    {
        Value = value;
    }

    public void AddEdge(GraphNode<T> to, int weight)
    {
        Edges.Add(new GraphEdge<T>(this, to, weight));
    }

    public override string ToString() => Value.ToString()!;
}