using Arc;
using Pastel;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ArcInstance
{
    public partial class Instance
    {
        public static readonly string directory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly Dictionary<string, string> Localisation = new();
        public static readonly List<string> Vanilla = new();
        public static string[] args;
        public static readonly string headers = $@"
/replace p@ with provinces:
/replace c@ with countries:
/replace a@ with areas:
/replace r@ with regions:
/replace s@ with superregions:
";
        public void Run(string[] args)
        {
            Instance.args = args;
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en");

            Compiler.directory = directory;
            Compiler.owner = this;



            string[] locations = File.ReadAllLines(Path.Combine(Compiler.directory, "arc.defines"));

            foreach (string location in locations)
            {
                TranspileTarget(location);
                Console.WriteLine($"Finished Loading: {location}".Pastel(ConsoleColor.Yellow));
            }

            ReligionGroup.Transpile();
            PersonalDeity.Transpile();
            TranspileAdjacencies();
            TranspileAreas();
            TranspileBookmarks();
            TranspileRegions();
            TranspileSuperregions();
            TranspileProvinces();
            TranspileTradeGoods();
            TranspileOnActions();
            TranspileTerrains();
            Blessing.Transpile();
            Building.Transpile();
            TranspileCountries();
            ChurchAspect.Transpile();
            AdvisorType.Transpile();
            TradeNode.Transpile();

            TranspileLocalisations();

            return;
        }
        public static void OverwriteFile(string path, string text, bool AllowFormatting = true)
        {
            path = Path.Combine(directory, path);
            if (AllowFormatting && args.Contains("--format")) text = Parser.FormatCode(text);

            if(File.Exists(path))
            {
                string old = File.ReadAllText(path);
                if(text != old) File.WriteAllText(path, text);
            }
            else
            {
                File.WriteAllText(path, text);
            }
        }
        private static void TranspileCountries()
        {
            StringBuilder sb = new();
            foreach(Country ctr in Country.Countries.Values())
            {
                Compiler comp = new();
                sb.Append($"{ctr.Tag} = \"countries/{ctr.Tag}.txt\"\n");
                OverwriteFile($"target/common/countries/{ctr.Tag}.txt", comp.Compile($"graphical_culture = {ctr.GraphicalCulture} color = {{ {ctr.Color} }} historical_idea_groups = {{ {ctr.HistoricalIdeaGroups} }} historical_units = {{ {ctr.HistoricalUnits} }} monarch_names = {{ {ctr.MonarchNames} }} leader_names = {{ {ctr.LeaderNames} }} ship_names = {{ {ctr.ShipNames} }} army_names = {{ {ctr.ArmyNames} }} fleet_names = {{ {ctr.FleetNames} }} {ctr.Definitions}"));
                OverwriteFile($"target/history/countries/{ctr.Tag}.txt", comp.Compile($"government = {ctr.Government} government_rank = {ctr.GovernmentRank} mercantilism = {ctr.Mercantilism} technology_group = {ctr.TechnologyGroup} religion = {ctr.Religion} primary_culture = {ctr.PrimaryCulture} capital = {ctr.Capital.Id}  {ctr.History}"));
                Localisation.Add($"{ctr.Tag}", $"{ctr.Name}");
                Localisation.Add($"{ctr.Tag}_ADJ", $"{ctr.Adj}");
            }
            OverwriteFile("target/common/country_tags/countries.txt", sb.ToString());
            Console.WriteLine($"Finished Transpiling Countries".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileAdjacencies()
        {
            StringBuilder sb = new("From;To;Type;Through;start_x;start_y;stop_x;stop_y;Comment;\n");
            foreach(Adjacency adjacency in Adjacency.Adjacencies.Values())
            {
                sb.Append($"{adjacency.From.Id};{adjacency.To.Id};{adjacency.Type};{adjacency.Through.Id};{adjacency.StartX};{adjacency.StartY};{adjacency.StopX};{adjacency.StopY};;\n");
            }
            sb.Append("-1;-1;;-1;-1;-1;-1;-1;-1;");
            OverwriteFile("target/map/adjacencies.csv", sb.ToString(), false);
            Console.WriteLine($"Finished Transpiling Adjacencies".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileBookmarks()
        {
            int i = 0;
            foreach (Bookmark bookmark in Bookmark.Bookmarks.Values())
            {
                StringBuilder sb = new("");
                Localisation.Add($"{bookmark.Id}_name", bookmark.Name.Value);
                Localisation.Add($"{bookmark.Id}_desc", bookmark.Desc.Value);
                sb.Append($"bookmark = {{ name = {bookmark.Id}_name desc = {bookmark.Id}_desc date = {bookmark.Date} center = {bookmark.Center.Id} ");
                foreach(Country? country in bookmark.Countries.Values)
                {
                    if (country == null) continue;
                    sb.Append($"country = {country.Tag} ");
                }
                foreach(Country? country in bookmark.EasyCountries.Values)
                {
                    if (country == null) continue;
                    sb.Append($"easy_country = {country.Tag} ");
                }
                if (bookmark.Default) sb.Append("default = yes ");
                if (!bookmark.Effect.IsEmpty()) sb.Append($"effect = {{ {bookmark.Effect.Compile()} }} ");
                sb.Append($"}} ");
                OverwriteFile($"target/common/bookmarks/{bookmark.Id}.txt", sb.ToString());
            }
            Console.WriteLine($"Finished Transpiling Bookmarks".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileLocalisations()
        {
            StringBuilder sb = new("l_english:\n");
            foreach (KeyValuePair<string, string> loc in Localisation)
            {
                sb.Append($" {loc.Key}: \"{loc.Value.Trim('"')}\"\n");
            }
            OverwriteFile("target/localisation/replace/arc5_l_english.yml", Parser.ConvertStringToUtf8Bom(sb.ToString()));
            Console.WriteLine($"Finished Transpiling Localisations".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileTerrains()
        {
            StringBuilder sb = new("categories = { pti = { type = pti } ");
            foreach (KeyValuePair<string, Terrain> terrain in Terrain.Terrains)
            {
                sb.Append($"{terrain.Key} = {{ color = {{ {terrain.Value.Color} }} sound_type = {terrain.Value.SoundType} {(terrain.Value.IsWater ? "is_water = yes" : "")}  {(terrain.Value.InlandSea ? "inland_sea = yes" : "")} {(terrain.Value.Type != null ? $"type = {terrain.Value.Type}" : "")} movement_cost = {terrain.Value.MovementCost} {(terrain.Value.Defence.Value == 0 ? "" : $"defence = {terrain.Value.Defence}")} {terrain.Value.Modifier.Compile()} terrain_override = {{ {(string.Join(' ', from Province in Province.Provinces.Values() where Province.Terrain == terrain.Value select Province.Id))} }} }} ");
                Localisation.Add(terrain.Value.Id.Value, terrain.Value.Name.Value);
                Localisation.Add($"{terrain.Value.Id}_desc", terrain.Value.Description.Value);
            }
            sb.Append(" } ");
            sb.Append($"terrain = {{ {Compiler.global["terrain_declarations"]} }}");
            sb.Append($"tree = {{ {Compiler.global["tree"]} }}");
            OverwriteFile("target/map/terrain.txt", sb.ToString());
            Console.WriteLine($"Finished Transpiling Terrains".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileOnActions()
        {
            StringBuilder sb = new();
            foreach (KeyValuePair<string, ArcBlock> OnAction in ((Dict<ArcBlock>)Compiler.global["on_actions"]))
            {
                sb.Append($"{OnAction.Key} = {{ {OnAction.Value.Compile()}}} ");
            }
            OverwriteFile("target/common/on_actions/00_on_actions.txt", sb.ToString());
            Console.WriteLine($"Finished Transpiling On Actions".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileTradeGoods()
        {
            StringBuilder sb = new("unknown = { color = { 0.5 0.5 0.5 } } ");
            StringBuilder sa = new("unknown = { base_price = 0 } ");
            foreach (TradeGood tradeGood in TradeGood.TradeGoods.Values())
            {
                sb.Append($"{tradeGood.Id} = {{ color = {{ {tradeGood.Color} }} modifier = {{ {tradeGood.Modifier.Compile()} }} province = {{ {tradeGood.Province.Compile()} }} chance = {{ {tradeGood.Chance} }} }} ");
                sa.Append($"{tradeGood.Id} = {{ base_price = {tradeGood.BasePrice} goldtype = {tradeGood.IsGold} }} ");
                Localisation.Add(tradeGood.Id.Value, tradeGood.Name.Value);
                Localisation.Add($"{tradeGood.Id}DESC", tradeGood.Description.Value);
            }
            OverwriteFile("target/common/tradegoods/00_tradegoods.txt", sb.ToString());
            OverwriteFile("target/common/prices/00_prices.txt", sa.ToString());
            Console.WriteLine($"Finished Transpiling Trade Goods".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileSuperregions()
        {
            StringBuilder sb = new();
            foreach (Superregion superregion in Superregion.Superregions.Values())
            {
                sb.Append($"{superregion.Id} = {{ {string.Join(' ', from Region in Region.Regions.Values() where Region.Superregion == superregion select Region.Id)} }} ");
                Localisation.Add($"{superregion.Id.Value}", superregion.Name.Value);
                Localisation.Add($"{superregion.Id.Value}_name", superregion.Name.Value);
                Localisation.Add($"{superregion.Id.Value}_adj", superregion.Adj.Value);
            }
            OverwriteFile("target/map/superregion.txt", sb.ToString());
            Console.WriteLine($"Finished Transpiling Superregion".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileRegions()
        {
            StringBuilder sb = new();
            foreach (Region region in Region.Regions.Values())
            {
                sb.Append($"{region.Id} = {{ areas = {{ {string.Join(' ', from Area in Area.Areas.Values() where Area.Region == region select Area.Id)} }} }} ");
                Localisation.Add($"{region.Id.Value}", region.Name.Value);
                Localisation.Add($"{region.Id.Value}_name", region.Name.Value);
                Localisation.Add($"{region.Id.Value}_adj", region.Adj.Value);
            }
            OverwriteFile("target/map/region.txt", sb.ToString());
            Console.WriteLine($"Finished Transpiling Regions".Pastel(ConsoleColor.Cyan));
        }
        private static void TranspileAreas()
        {
            StringBuilder sb = new();
            foreach (Area area in Area.Areas.Values())
            {
                sb.Append($"{area.Id} = {{ {string.Join(' ', from Province in Province.Provinces.Values() where Province.Area == area select Province.Id)} }} ");
                Localisation.Add($"{area.Id.Value}", area.Name.Value);
            }
            OverwriteFile("target/map/area.txt", sb.ToString());
            Console.WriteLine($"Finished Transpiling Areas".Pastel(ConsoleColor.Cyan));
        }
        public static void TranspileTarget(string path)
        {
            string fileLocation = Path.Combine(directory, path);

            if (fileLocation.EndsWith("/"))
            {
                string[] files = Directory.GetFiles(fileLocation);
                foreach (string file in files)
                {
                    try
                    {
                        Compiler comp = new();
                        string fileContent = File.ReadAllText(file);
                        comp.Compile(fileContent + headers, true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{Path.GetRelativePath(directory, file)}: {e}".Pastel(ConsoleColor.Red));
                        throw;
                    }
                }
            }
            else
            {
                //try
                //{
                    Compiler comp = new();
                    string file = File.ReadAllText(fileLocation);
                    comp.Compile(file + headers, true);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine($"{Path.GetRelativePath(directory, fileLocation)}: {e}".Pastel(ConsoleColor.Red));
                //    throw e;
                //}
            }
        }
        private static void TranspileProvinces()
        {
            Compiler comp = new();
            StringBuilder Positions = new();
            StringBuilder ProvinceDefines = new();
            StringBuilder Impassibles = new();
            StringBuilder SeaTiles = new();
            StringBuilder LakeTiles = new();
            StringBuilder Continent = new("Tamriel = {");
            foreach (KeyValuePair<string, Province> province in Province.Provinces)
            {
                int id = province.Value.Id.Value;
                string name = province.Value.Name.Value;
                Block color = province.Value.Color.Value;
                Block history = province.Value.History.Value;
                Localisation.Add($"PROV{id}", name);
                Localisation.Add($"PROV_ADJ{id}", name);
                ProvinceDefines.Append($"{id};{string.Join(';', color)};;x\n");

                if (province.Value.Impassible.Value)
                {
                    Impassibles.Append($"{id} ");
                }
                if (province.Value.Sea.Value)
                {
                    SeaTiles.Append($"{id} ");
                }
                if (province.Value.Lake.Value)
                {
                    LakeTiles.Append($"{id} ");
                }

                string result = "";
                if (history.Count > 0) result = (province.Value.IsLand() ? SplitToDev(province.Value.BaseDevelopment.Value) : "") + comp.Compile(history);

                Positions.Append($"{province.Value.Id} = {{ position = {{ {province.Value.Position} }} rotation = {{ {province.Value.Rotation} }} height = {{ {province.Value.Height} }} }} ");

                if (province.Value.IsLand() || province.Value.Impassible) Continent.Append($" {province.Value.Id}");

                OverwriteFile($"target/history/provinces/{id} - ARC.txt", result);
            }
            Continent.Append(" }");

            string SplitToDev(int i)
            {
                int quotient = i / 3;
                int remainder = i % 3;

                int first = quotient + (remainder >= 1 ? 1 : 0);
                int second = quotient + (remainder >= 2 ? 1 : 0);
                int third = quotient;

                return $"base_tax = {first} base_production = {second} base_manpower = {third} ";
            }

            OverwriteFile("target/map/continent.txt", Continent.ToString());
            OverwriteFile("target/map/positions.txt", Positions.ToString());
            OverwriteFile("target/map/definition.csv", ProvinceDefines.ToString(), false);
            OverwriteFile("target/map/climate.txt", $@"
tropical = {{
    
}}

arid = {{
    
}}

arctic = {{
	
}}

mild_winter = {{
    
}}


normal_winter = {{
    
}}

severe_winter = {{
    
}}

impassable = {{
	{Impassibles}
}}

mild_monsoon = {{
    
}}

normal_monsoon = {{
    
}}

severe_monsoon = {{
    
}}

equator_y_on_province_image = 224");
            OverwriteFile("target/map/default.map", $@"
width = 4096
height = 2816

max_provinces = {Province.Provinces.Count + 1}
sea_starts = {{ 
	{SeaTiles}
}}

only_used_for_random = {{
}}

lakes = {{
	{LakeTiles}
}}

force_coastal = {{
}}

definitions = ""definition.csv""
provinces = ""provinces.bmp""
positions = ""positions.txt""
terrain = ""terrain.bmp""
rivers = ""rivers.bmp""
terrain_definition = ""terrain.txt""
heightmap = ""heightmap.bmp""
tree_definition = ""trees.bmp""
continent = ""continent.txt""
adjacencies = ""adjacencies.csv""
climate = ""climate.txt""
region = ""region.txt""
superregion = ""superregion.txt""
area = ""area.txt""
provincegroup = ""provincegroup.txt""
ambient_object = ""ambient_object.txt""
seasons = ""seasons.txt""
trade_winds = ""trade_winds.txt""

# Define which indices in trees.bmp palette which should count as trees for automatic terrain assignment
tree = {{ 3 4 7 10 }}
");
            Console.WriteLine($"Finished Transpiling Provinces".Pastel(ConsoleColor.Cyan));
        }
    }
}
