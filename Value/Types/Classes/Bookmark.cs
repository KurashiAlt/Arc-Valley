using ArcInstance;
using System.Text;

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
    public ArcEffect Effect { get; set; }

    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Bookmark(ArcString id, ArcString name, ArcString desc, ArcString date, Province center, ArcList<Country> countries, ArcList<Country> easyCountries, ArcBool @default, ArcEffect effect)
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
            args.GetDefault(ArcEffect.Constructor, "effect", new())
        );

        Bookmarks.Add(id, bookmark);

        return i;
    }
    public static string Transpile()
    {
        int i = 0;
        foreach (Bookmark bookmark in Bookmarks.Values())
        {
            StringBuilder sb = new("");
            Instance.Localisation.Add($"{bookmark.Id}_name", bookmark.Name.Value);
            Instance.Localisation.Add($"{bookmark.Id}_desc", bookmark.Desc.Value);
            sb.Append($"bookmark = {{ name = {bookmark.Id}_name desc = {bookmark.Id}_desc date = {bookmark.Date} center = {bookmark.Center.Id} ");
            foreach (Country? country in bookmark.Countries.Values)
            {
                if (country == null) continue;
                sb.Append($"country = {country.Tag} ");
            }
            foreach (Country? country in bookmark.EasyCountries.Values)
            {
                if (country == null) continue;
                sb.Append($"easy_country = {country.Tag} ");
            }
            if (bookmark.Default) sb.Append("default = yes ");
            if (!bookmark.Effect.IsEmpty()) sb.Append($"effect = {{ {bookmark.Effect.Compile()} }} ");
            sb.Append($"}} ");
            Instance.OverwriteFile($"{Instance.TranspileTarget}/common/bookmarks/{bookmark.Id}.txt", sb.ToString());
        }
        return "Bookmarks";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
