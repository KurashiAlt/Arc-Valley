using ArcInstance;
using System.Text;

namespace Arc;
public class TradeGood : IArcObject
{
    public static readonly Dict<TradeGood> TradeGoods = new();
    public bool IsObject() => true;
    public string Class => "Tradegood";
    public ArcString Name { get; set; }
    public ArcString Description { get; set; }
    public ArcBlock Color { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcBlock Province { get; set; }
    public ArcFloat BasePrice { get; set; }
    public ArcBool IsGold { get; set; }
    public ArcBlock Chance { get; set; }
    public ArcString Id { get; set; }
    public Dict<IVariable> KeyValuePairs { get; set; }
    public TradeGood(ArcString Name, ArcString Description, ArcBlock Color, ArcBlock Modifier, ArcBlock Province, ArcFloat BasePrice, ArcBool IsGold, ArcBlock Chance, ArcString Id)
    {
        this.Name = Name;
        this.Description = Description;
        this.Color = Color;
        this.Modifier = Modifier;
        this.Province = Province;
        this.BasePrice = BasePrice;
        this.IsGold = IsGold;
        this.Chance = Chance;
        this.Id = Id;
        KeyValuePairs = new()
        {
            { "name", Name },
            { "desc", Description },
            { "color", Color },
            { "modifier", Modifier },
            { "province", Province },
            { "base_price", BasePrice },
            { "is_gold", IsGold },
            { "chance", Chance },
            { "id", Id }
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        TradeGood TradeGood = new(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcCode.Constructor, "color"),
            args.Get(ArcModifier.Constructor, "modifier"),
            args.Get(ArcModifier.Constructor, "province"),
            args.Get(ArcFloat.Constructor, "base_price"),
            args.Get(ArcBool.Constructor, "is_gold"),
            args.Get(ArcCode.Constructor, "chance"),
            new(id)
        );

        TradeGoods.Add(id, TradeGood);

        return i;
    }
    public override string ToString() => Id.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
    public static string Transpile()
    {
        StringBuilder sb = new("unknown = { color = { 0.5 0.5 0.5 } } ");
        StringBuilder sa = new("unknown = { base_price = 0 } ");
        foreach (TradeGood tradeGood in TradeGood.TradeGoods.Values())
        {
            sb.Append($"{tradeGood.Id} = {{ color = {{ {tradeGood.Color} }} modifier = {{ {tradeGood.Modifier.Compile()} }} province = {{ {tradeGood.Province.Compile()} }} chance = {{ {tradeGood.Chance} }} }} ");
            sa.Append($"{tradeGood.Id} = {{ base_price = {tradeGood.BasePrice} goldtype = {tradeGood.IsGold} }} ");
            Instance.Localisation.Add(tradeGood.Id.Value, tradeGood.Name.Value);
            Instance.Localisation.Add($"{tradeGood.Id}DESC", tradeGood.Description.Value);
        }
        Instance.OverwriteFile("target/common/tradegoods/00_tradegoods.txt", sb.ToString());
        Instance.OverwriteFile("target/common/prices/00_prices.txt", sa.ToString());
        return "Trade Goods";
    }
}