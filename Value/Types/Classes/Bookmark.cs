namespace Arc;
public class Bookmark : IArcObject
{
    public static readonly Dict<Bookmark> Bookmarks = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Date { get; set; }
    public Province Center { get; set; }
    public ArcList<Country> Countries { get; set; }
    public ArcList<Country> EasyCountries { get; set; }
    public ArcBool Default { get; set; }
    public ArcBlock Effect { get; set; }

    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Bookmark(ArcString id, ArcString name, ArcString desc, ArcString date, Province center, ArcList<Country> countries, ArcList<Country> easyCountries, ArcBool @default, ArcBlock effect)
    {
        Id = id;
        Name = name;
        Desc = desc;
        Date = date;
        Center = center;
        Countries = countries;
        EasyCountries = easyCountries;
        Default = @default;
        Effect = effect;
        KeyValuePairs = new() 
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "center", Center },
            { "countries", Countries },
            { "easy_countries", EasyCountries },
            { "default", Default },
            { "effect", Effect },
        };
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);
        Bookmark bookmark = new(
            new(id),
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcString.Constructor, "date"),
            args.GetFromList(Province.Provinces, "center"),
            args.GetDefault((Block s) => new ArcList<Country>(s, Country.Countries), "countries", new()),
            args.GetDefault((Block s) => new ArcList<Country>(s, Country.Countries), "easy_countries", new()),
            args.GetDefault(ArcBool.Constructor, "default", new(false)),
            args.GetDefault(ArcBlock.Constructor, "effect", new())
        );

        Bookmarks.Add(id, bookmark);

        return i;
    }

    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
}
