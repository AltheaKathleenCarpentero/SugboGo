using SugboGo.Models;

namespace SugboGo.Data;

public static class TravelSpotSeedData
{
    public static IReadOnlyList<TravelSpot> GetTravelSpots()
    {
        TravelSpot[] spots =
        [
        new() { Id = 1, Name = "Magellan's Cross", Location = "Cebu City", Description = "Historical cross planted by Ferdinand Magellan in 1521.", Category = "Historical", Region = "Cebu City", IsPopular = true },
        new() { Id = 2, Name = "Basilica del Santo Nino", Location = "Cebu City", Description = "Oldest Roman Catholic church in the Philippines.", Category = "Religious", Region = "Cebu City", IsPopular = true },
        new() { Id = 3, Name = "Fort San Pedro", Location = "Cebu City", Description = "Smallest triangular fort in the Philippines.", Category = "Historical", Region = "Cebu City", IsPopular = true },
        new() { Id = 4, Name = "Cebu Taoist Temple", Location = "Beverly Hills, Cebu City", Description = "Scenic Taoist temple with city views.", Category = "Religious", Region = "Cebu City", IsPopular = true },
        new() { Id = 5, Name = "Temple of Leah", Location = "Busay, Cebu City", Description = "Grand monument of love with European architecture.", Category = "Monument", Region = "Cebu City", IsPopular = true },
        new() { Id = 6, Name = "Sirao Garden", Location = "Busay, Cebu City", Description = "Flower garden known as Little Amsterdam.", Category = "Garden", Region = "Cebu City", IsPopular = true },
        new() { Id = 7, Name = "Tops Lookout", Location = "Busay, Cebu City", Description = "Panoramic viewpoint of Cebu City.", Category = "Viewpoint", Region = "Cebu City", IsPopular = true },
        new() { Id = 8, Name = "Heritage of Cebu Monument", Location = "Parian, Cebu City", Description = "Sculptural tableau of Cebu history.", Category = "Monument", Region = "Cebu City" },
        new() { Id = 9, Name = "Yap-Sandiego Ancestral House", Location = "Cebu City", Description = "Oldest ancestral house in Cebu.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 10, Name = "Casa Gorordo Museum", Location = "Cebu City", Description = "Restored 19th-century house museum.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 11, Name = "Cebu Metropolitan Cathedral", Location = "Cebu City", Description = "Historic cathedral in the city center.", Category = "Religious", Region = "Cebu City" },
        new() { Id = 12, Name = "Fuente Osmena Circle", Location = "Cebu City", Description = "Iconic roundabout and park.", Category = "Park", Region = "Cebu City" },
        new() { Id = 13, Name = "Colon Street", Location = "Cebu City", Description = "Oldest street in the Philippines.", Category = "Street", Region = "Cebu City" },
        new() { Id = 14, Name = "Mactan Shrine", Location = "Lapu-Lapu City", Description = "Monument to Lapu-Lapu.", Category = "Historical", Region = "Mactan" },
        new() { Id = 15, Name = "National Museum of the Philippines - Cebu", Location = "Cebu City", Description = "Showcases Cebu heritage and artifacts.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 16, Name = "Cebu Provincial Capitol", Location = "Cebu City", Description = "Neoclassical government building.", Category = "Landmark", Region = "Cebu City" },
        new() { Id = 17, Name = "Ayala Center Cebu", Location = "Cebu City", Description = "Major shopping mall.", Category = "Shopping", Region = "Cebu City" },
        new() { Id = 18, Name = "SM Seaside City Cebu", Location = "Cebu City", Description = "Large mall and entertainment complex.", Category = "Shopping", Region = "Cebu City" },
        new() { Id = 19, Name = "Cebu IT Park", Location = "Cebu City", Description = "Modern business and lifestyle district.", Category = "Urban", Region = "Cebu City" },
        new() { Id = 20, Name = "Cebu Ocean Park", Location = "Cebu City", Description = "Marine theme park.", Category = "Wildlife", Region = "Cebu City" },
        new() { Id = 21, Name = "Carbon Market", Location = "Cebu City", Description = "Historic public market and local food stop.", Category = "Market", Region = "Cebu City" },
        new() { Id = 22, Name = "10,000 Roses Cafe", Location = "Cordova", Description = "Seaside cafe known for illuminated white rose installations.", Category = "Cafe", Region = "Mactan" },
        new() { Id = 23, Name = "Anjo World Theme Park", Location = "Minglanilla", Description = "Theme park with rides and family attractions.", Category = "Theme Park", Region = "Metro Cebu" },

        new() { Id = 51, Name = "Kawasan Falls", Location = "Badian", Description = "Iconic turquoise multi-tiered waterfalls.", Category = "Waterfall", Region = "South Cebu", IsPopular = true },
        new() { Id = 52, Name = "Inambakan Falls", Location = "Alegria", Description = "Multi-level falls with swimming and trekking.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 53, Name = "Mantayupan Falls", Location = "Barangay Mantayupan", Description = "Tall powerful waterfall with pools.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 54, Name = "Tumalog Falls", Location = "Oslob", Description = "Curtain-like waterfall.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 55, Name = "Aguinid Falls", Location = "Samboan", Description = "Series of cascading falls.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 56, Name = "Kabutongan Falls", Location = "South Cebu", Description = "Deep blue pool with cave and cliff jump.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 57, Name = "Dao Falls", Location = "South Cebu", Description = "Epic canyon hike waterfall.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 58, Name = "Cambais Falls", Location = "South Cebu", Description = "Scenic falls for swimming.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 59, Name = "Binalayan Hidden Falls", Location = "South Cebu", Description = "Claw-like cascades for cliff diving.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 60, Name = "Cancalanog Falls", Location = "South Cebu", Description = "Off-the-beaten-path stunning falls.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 61, Name = "Lusno Falls", Location = "Ronda", Description = "Local waterfall known for swimming and a quieter rural setting.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 62, Name = "Tinubdan Falls", Location = "Catmon", Description = "Northern Cebu waterfall with natural pools.", Category = "Waterfall", Region = "North Cebu" },
        new() { Id = 63, Name = "Kanlaob River Falls", Location = "Alegria", Description = "River canyon waterfall route used for trekking and water activities.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 64, Name = "Bugnawan Falls", Location = "South Cebu", Description = "Local waterfall stop for swimming and nature trips.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 65, Name = "Kampael Falls", Location = "South Cebu", Description = "Scenic local waterfall for adventure itineraries.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 66, Name = "Bucawe Falls", Location = "South Cebu", Description = "Hidden waterfall suited for local-guided routes.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 67, Name = "Montpeller Falls", Location = "Alegria", Description = "Waterfall and natural pool experience in South Cebu.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 68, Name = "Dau Falls", Location = "Samboan", Description = "Tall waterfall reached by a scenic river trek.", Category = "Waterfall", Region = "South Cebu" },

        new() { Id = 151, Name = "Panagsama Beach", Location = "Moalboal", Description = "Famous for sardine run and diving.", Category = "Beach", Region = "South Cebu", IsPopular = true },
        new() { Id = 152, Name = "Sumilon Island", Location = "Oslob", Description = "White sandbar and marine sanctuary.", Category = "Island", Region = "South Cebu", IsPopular = true },
        new() { Id = 153, Name = "Bantayan Island", Location = "North Cebu", Description = "Pristine beaches and relaxed vibe.", Category = "Island", Region = "North Cebu" },
        new() { Id = 154, Name = "Malapascua Island", Location = "North Cebu", Description = "Diving paradise with thresher sharks.", Category = "Island", Region = "North Cebu", IsPopular = true },
        new() { Id = 155, Name = "Lambug Beach", Location = "Badian", Description = "Quiet white sand beach.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 156, Name = "Tingko Beach", Location = "Alcoy", Description = "Scenic beach in south Cebu.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 157, Name = "Hermit's Cove", Location = "Aloguinsan", Description = "Hidden picturesque cove.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 158, Name = "Basdaku Beach", Location = "Moalboal", Description = "White sand beach known for longer stays and group trips.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 159, Name = "Carnaza Island", Location = "North Cebu", Description = "Untouched island beauty.", Category = "Island", Region = "North Cebu" },
        new() { Id = 160, Name = "Pescador Island", Location = "Moalboal", Description = "Great for snorkeling and diving.", Category = "Island", Region = "South Cebu" },
        new() { Id = 161, Name = "Bounty Beach", Location = "Malapascua", Description = "Beachfront area popular with divers visiting Malapascua.", Category = "Beach", Region = "North Cebu" },
        new() { Id = 162, Name = "Paradise Beach", Location = "Bantayan Island", Description = "Quiet beach stop on Bantayan Island.", Category = "Beach", Region = "North Cebu" },
        new() { Id = 163, Name = "White Beach Moalboal", Location = "Moalboal", Description = "Long beach area used for swimming and sunsets.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 164, Name = "Vano Beach", Location = "Mactan", Description = "Accessible beach area in Mactan.", Category = "Beach", Region = "Mactan" },
        new() { Id = 165, Name = "Langub Beach", Location = "Malapascua", Description = "Quiet beach area on Malapascua Island.", Category = "Beach", Region = "North Cebu" },
        new() { Id = 166, Name = "Santiago Bay", Location = "Camotes Islands", Description = "Wide beach bay in Camotes.", Category = "Beach", Region = "Camotes" },
        new() { Id = 167, Name = "Olango Island", Location = "Lapu-Lapu City", Description = "Island known for bird sanctuary visits and coastal routes.", Category = "Island", Region = "Mactan" },

        new() { Id = 301, Name = "Osmena Peak", Location = "Dalaguete", Description = "Highest point in Cebu with dramatic karst views.", Category = "Mountain", Region = "South Cebu", IsPopular = true },
        new() { Id = 302, Name = "Casino Peak (Lugsangan Peak)", Location = "Dalaguete", Description = "Chocolate Hills-like limestone formations.", Category = "Viewpoint", Region = "South Cebu" },
        new() { Id = 303, Name = "Kandungaw Peak", Location = "South Cebu", Description = "Lesser-known hiking peak.", Category = "Mountain", Region = "South Cebu" },
        new() { Id = 304, Name = "Mt. Naupa", Location = "Naga", Description = "Accessible mountain trail with ridge views.", Category = "Mountain", Region = "South Cebu" },
        new() { Id = 305, Name = "Mt. Kan-Irag (Sirao Peak)", Location = "Cebu City", Description = "Highland hiking route near Sirao.", Category = "Mountain", Region = "Cebu City" },
        new() { Id = 306, Name = "Mt. Babag", Location = "Cebu City", Description = "Mountain trail and viewpoint near the city.", Category = "Mountain", Region = "Cebu City" },
        new() { Id = 307, Name = "Bocaue Peak", Location = "Cebu", Description = "Viewpoint and local hiking destination.", Category = "Viewpoint", Region = "Cebu" },
        new() { Id = 308, Name = "Dalas-ag Peak", Location = "Cebu", Description = "Scenic mountain peak for guided hikes.", Category = "Mountain", Region = "Cebu" },

        new() { Id = 401, Name = "Oslob Whale Sharks", Location = "Oslob", Description = "Swim with gentle whale sharks.", Category = "Wildlife", Region = "South Cebu", IsPopular = true },
        new() { Id = 402, Name = "Cebu Safari and Adventure Park", Location = "Carmen", Description = "Wildlife park with adventure activities.", Category = "Wildlife", Region = "North Cebu" },
        new() { Id = 403, Name = "Simala Shrine", Location = "Sibonga", Description = "Castle-like pilgrimage church and shrine.", Category = "Religious", Region = "South Cebu", IsPopular = true },
        new() { Id = 404, Name = "Bojo River Eco Tour", Location = "Aloguinsan", Description = "Community-led river cruise and eco-tour experience.", Category = "Eco Tour", Region = "South Cebu" },
        new() { Id = 405, Name = "Don Emilio Botanical Garden", Location = "Busay, Cebu City", Description = "Botanical garden and mountain leisure stop.", Category = "Garden", Region = "Cebu City" },
        new() { Id = 406, Name = "Carcar Heritage Houses", Location = "Carcar", Description = "Ancestral houses and heritage district in Carcar.", Category = "Historical", Region = "South Cebu" },
        new() { Id = 407, Name = "Argao Church", Location = "Argao", Description = "Historic church in southern Cebu.", Category = "Religious", Region = "South Cebu" },
        new() { Id = 408, Name = "Boljoon Church", Location = "Boljoon", Description = "Historic church and heritage landmark.", Category = "Religious", Region = "South Cebu" },
        new() { Id = 409, Name = "Sky Adventure Cebu", Location = "Cebu City", Description = "", Category = "Theme Park", Region = "Cebu City" },
        new() { Id = 410, Name = "Mountain View Nature Park", Location = "Cebu", Description = "", Category = "Nature Park", Region = "Cebu" },
        new() { Id = 411, Name = "Neri’s Ville", Location = "Cebu", Description = "", Category = "Resort", Region = "Cebu" },
        new() { Id = 412, Name = "Strawberry de Cantipla Eco Farm", Location = "Cebu City", Description = "", Category = "Farm", Region = "Cebu City" },
        new() { Id = 413, Name = "Terrazas de Flores", Location = "Cebu", Description = "", Category = "Garden", Region = "Cebu" },
        new() { Id = 414, Name = "Museo Sugbo", Location = "Cebu City", Description = "", Category = "Museum", Region = "Cebu City" },
        new() { Id = 415, Name = "1730 Jesuit House", Location = "Cebu City", Description = "", Category = "Historical", Region = "Cebu City" },
        new() { Id = 416, Name = "Sugbo Mercado", Location = "Cebu City", Description = "", Category = "Market", Region = "Cebu City" },
        new() { Id = 417, Name = "The Pyramid", Location = "Cebu City", Description = "(closed)", Category = "Landmark", Region = "Cebu City" },
        new() { Id = 418, Name = "La Vie Parisienne", Location = "Cebu", Description = "", Category = "Cafe", Region = "Cebu" },
        new() { Id = 419, Name = "La Vie in the Sky", Location = "Cebu", Description = "", Category = "Restaurant", Region = "Cebu" },
        new() { Id = 420, Name = "Mist Mountain Resort", Location = "Cebu", Description = "", Category = "Resort", Region = "Cebu" },
        new() { Id = 421, Name = "Waterfront Hotel", Location = "Cebu City", Description = "", Category = "Hotel", Region = "Cebu City" },
        new() { Id = 422, Name = "Top of Cebu", Location = "Cebu", Description = "", Category = "Viewpoint", Region = "Cebu" },
        new() { Id = 423, Name = "Lantaw Native Restaurant", Location = "Cordova", Description = "", Category = "Restaurant", Region = "Mactan" },
        new() { Id = 424, Name = "Kabang Falls (Budlaan)", Location = "Cebu City", Description = "", Category = "Waterfall", Region = "Cebu City" },
        new() { Id = 425, Name = "Himbabawod Falls (Bonbon)", Location = "Cebu", Description = "", Category = "Waterfall", Region = "Cebu" },
        new() { Id = 426, Name = "Busay Lut-od Falls", Location = "Cebu", Description = "", Category = "Waterfall", Region = "Cebu" },
        new() { Id = 427, Name = "Kawa Falls (Toong Pardo)", Location = "Cebu", Description = "", Category = "Waterfall", Region = "Cebu" },
        new() { Id = 428, Name = "NUSTAR Resort & Casino Cebu", Location = "Cebu City", Description = "", Category = "Resort", Region = "Cebu City" },
        new() { Id = 429, Name = "SM Seaside Sky Park", Location = "Cebu City", Description = "", Category = "Park", Region = "Cebu City" },
        new() { Id = 430, Name = "Il Corso Lifemalls", Location = "Cebu City", Description = "", Category = "Shopping", Region = "Cebu City" },
        new() { Id = 431, Name = "SRP Boardwalk", Location = "Cebu City", Description = "", Category = "Park", Region = "Cebu City" },
        new() { Id = 432, Name = "Cebu Fun Park", Location = "Cebu", Description = "", Category = "Theme Park", Region = "Cebu" },
        new() { Id = 433, Name = "ICON Cebu", Location = "Cebu City", Description = "", Category = "Shopping", Region = "Cebu City" },
        // === NEW ENTRIES (All missing spots from your list) ===
        // Lapu-Lapu City
        new() { Id = 434, Name = "Caohagan Island", Location = "Lapu-Lapu City", Description = "", Category = "Island", Region = "Mactan" },
        new() { Id = 435, Name = "Nalusuan Island", Location = "Lapu-Lapu City", Description = "", Category = "Island", Region = "Mactan" },
        new() { Id = 436, Name = "Pangan-an Island", Location = "Lapu-Lapu City", Description = "", Category = "Island", Region = "Mactan" },
        new() { Id = 437, Name = "Sulpa Islet", Location = "Lapu-Lapu City", Description = "", Category = "Island", Region = "Mactan" },
        new() { Id = 438, Name = "San Vicente Marine Sanctuary", Location = "Lapu-Lapu City", Description = "", Category = "Marine Sanctuary", Region = "Mactan" },
        new() { Id = 439, Name = "Yellow Submarine Dive Center", Location = "Mactan", Description = "", Category = "Diving", Region = "Mactan" },
        new() { Id = 440, Name = "Pawod Spring", Location = "Lapu-Lapu City", Description = "", Category = "Spring", Region = "Mactan" },
        new() { Id = 441, Name = "Shangri-La’s Mactan Resort & Spa", Location = "Mactan", Description = "", Category = "Resort", Region = "Mactan" },
        new() { Id = 442, Name = "Crimson Resort and Spa Mactan", Location = "Mactan", Description = "", Category = "Resort", Region = "Mactan" },
        new() { Id = 443, Name = "Plantation Bay Resort & Spa", Location = "Mactan", Description = "", Category = "Resort", Region = "Mactan" },
        new() { Id = 444, Name = "Dusit Thani Mactan Cebu", Location = "Mactan", Description = "", Category = "Resort", Region = "Mactan" },
        new() { Id = 445, Name = "JPark Island Resort & Waterpark", Location = "Mactan", Description = "", Category = "Resort", Region = "Mactan" },

        // Mandaue, Talisay, Naga, Carcar, Danao, etc.
        new() { Id = 446, Name = "Waterworld Cebu", Location = "Mandaue City", Description = "", Category = "Water Park", Region = "Mandaue" },
        new() { Id = 447, Name = "Upside Down World", Location = "Mandaue City", Description = "", Category = "Theme Park", Region = "Mandaue" },
        new() { Id = 448, Name = "Westown Lagoon", Location = "Mandaue City", Description = "", Category = "Park", Region = "Mandaue" },
        new() { Id = 449, Name = "Mandaue Heritage Plaza", Location = "Mandaue City", Description = "", Category = "Historical", Region = "Mandaue" },
        new() { Id = 450, Name = "Hidden Valley Mountain Resort", Location = "Talisay City", Description = "", Category = "Resort", Region = "Talisay" },
        new() { Id = 451, Name = "Crocolandia", Location = "Talisay City", Description = "", Category = "Wildlife", Region = "Talisay" },
        new() { Id = 452, Name = "Naga Boardwalk / Coastal Boulevard", Location = "Naga City", Description = "", Category = "Park", Region = "Naga" },
        new() { Id = 453, Name = "Carcar Heritage Town", Location = "Carcar City", Description = "", Category = "Historical", Region = "Carcar" },
        new() { Id = 454, Name = "Danao Adventure Park", Location = "Danao City", Description = "", Category = "Adventure", Region = "Danao" },
        new() { Id = 455, Name = "Danasan Eco Adventure Park", Location = "Danao City", Description = "", Category = "Adventure", Region = "Danao" },

        // More key additions (Bantayan, Camotes, etc.)
        new() { Id = 456, Name = "Kota Beach", Location = "Bantayan Island", Description = "", Category = "Beach", Region = "Bantayan" },
        new() { Id = 457, Name = "Ogtong Cave", Location = "Bantayan Island", Description = "", Category = "Cave", Region = "Bantayan" },
        new() { Id = 458, Name = "Lake Danao", Location = "Camotes Island", Description = "", Category = "Lake", Region = "Camotes" },
        new() { Id = 459, Name = "Bukilat Cave", Location = "Camotes Island", Description = "", Category = "Cave", Region = "Camotes" },
        new() { Id = 460, Name = "Gilutungan Island", Location = "Cordova", Description = "", Category = "Island", Region = "Mactan" },
    ];

        foreach (var spot in spots)
        {
            spot.ImageUrl = $"/images/{spot.Id}.jpg";
            
            // Assign Adventure Level based on Category
            spot.AdventureLevel = spot.Category switch
            {
                "Waterfall" => 4,
                "Island" => 3,
                "Mountain" => 5,
                "Beach" => 2,
                "Historical" => 1,
                "Religious" => 1,
                "Wildlife" => 3,
                "Theme Park" => 3,
                _ => 2
            };

            // Assign Base Price based on Category
            spot.BasePrice = spot.Category switch
            {
                "Island" => 3500m,
                "Waterfall" => 1800m,
                "Mountain" => 1200m,
                "Wildlife" => 2500m,
                "Theme Park" => 1500m,
                "Historical" => 800m,
                "Religious" => 500m,
                _ => 2000m
            };
        }

        return spots;
    }
}
