namespace aaspe_common.AasxCsharpLibrary.Extensions.Comparer;

public class ComparerIndexed : IComparer<IReferable>
{
    public int NullIndex = int.MaxValue;
    public readonly Dictionary<IReferable?, int> Index = new();

    public int Compare(IReferable? a, IReferable? b)
    {
        switch (a)
        {
            case null when b == null:
                return 0;
            case null:
                return 1;
        }

        if (b == null)
            return -1;

        var ca = Index.ContainsKey(a);
        var cb = Index.ContainsKey(b);

        switch (ca)
        {
            case false when !cb:
                return 0;
            // make CDs without usage to appear at end of list
            case false:
                return 1;
        }

        if (!cb)
            return -1;

        var ia = Index[a];
        var ib = Index[b];

        if (ia == ib)
            return 0;
        if (ia < ib)
            return -1;
        return 1;
    }
}