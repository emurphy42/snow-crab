using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowCrab
{
    class ModConfig
    {
        // https://github.com/veywrn/StardewValley/blob/master/StardewValley/Locations/MineShaft.cs
        // Rock levels: Roughly 15% of monsters are crabs
        // Lava levels: Roughly 10% of monsters are crabs
        // Will not replace ghosts (whose numbers are limited already)
        //   or skeletons (about 75% of monsters on levels 71-79 are skeletons, other 25% have similar distribution to 41-69)
        public double SnowCrabChance { get; set; } = .125;

        // https://stardewvalleywiki.com/Modding:Monster_data
        // https://stardewids.com/
        // https://github.com/veywrn/StardewValley/blob/master/StardewValley/Monsters/RockCrab.cs
        // https://github.com/veywrn/StardewValley/blob/master/StardewValley/Monsters/LavaCrab.cs

        // Health: Rock Crab = 30, Lava Crab = 120, Iridium Crab = 240
        public int SnowCrabHealth { get; set; } = 60;

        // DamageToFarmer: Rock Crab = 5, Lava Crab = 15, Iridium Crab = 15
        public int SnowCrabDamageToFarmer { get; set; } = 10;

        // Resilience: Rock Crab = 1, Lava Crab = 3
        public int SnowCrabResilience { get; set; } = 2;

        // ExperienceGained: Rock Crab = 4, Lava Crab = 12, Iridium Crab = 20
        public int SnowCrabExperienceGained { get; set; } = 8;

        // ObjectsToDrop:
        //   Rock Crab = 717 (Crab) .15 286 (Cherry Bomb) .4 96 (Dwarf Scroll I) .005 99 (Dwarf Scroll IV) .001
        //   Lava Crab = 717 (Crab) .25 287 (Bomb) .4 98 (Dwarf Scroll III) .005 99 (Dwarf Scroll IV) .001
        //   Iridium Crab = 732 (Crab Cakes) .5 386 (Iridium Ore)  .5 386 .5 386 .5
        public string SnowCrabObjectsToDrop { get; set; } = "717 .20 286 .2 287 .2 97 .005 99 .001"; // 97 = Dwarf Scroll II

        // https://github.com/veywrn/StardewValley/blob/master/StardewValley/Quests/SlayMonsterQuest.cs

        // Chance that a quest to kill a common frozen-levels monster is for snow crabs
        //   (normally 50/50 between blue slimes and dust spirits)
        public double SnowCrabQuestChance { get; set; } = .333;

        // Minimum/maximum number of snow crabs to kill for a quest: Rock Crab, Lava Crab = 2 to 5
        public int SnowCrabQuestMinimum { get; set; } = 2;
        public int SnowCrabQuestMaximum { get; set; } = 5;

        // Quest reward per snow crab killed: Rock Crab = 75, Lava Crab = 180
        public int SnowCrabQuestRewardPerKill { get; set; } = 125;

        // Setting this to true produces console output each time a snow crab is spawned
        public bool SnowCrabDebugOutput { get; set; } = false;
    }
}
