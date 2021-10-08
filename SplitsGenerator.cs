using System.Collections.Generic;

namespace dmc3music
{
    public class SplitsGenerator
    {
        public static List<string> missionNames = new List<string>()
        {
            "Run May Die",
            "Sickles",
            "Ice Age",
            "Cuntipede",
            "Backstabbers",
            "Trial of Error",
            "Foolishness",
            "Belly of the Beast",
            "Heavy Metal Cave",
            "Dante Must Deliver",
            "Berserk",
            "Timestomp",
            "The Runback",
            "Killer Bee",
            "The Quest for Aja",
            "Pretty Woman",
            "Rise and Shine",
            "Wily Stages",
            "Jelly",
            "Wanna Know the Name?"
        };

        public static List<string[]> roomsByMission = new List<string[]>()
        {
            // Mission 1
            new string[] {
                "Run May Die"
            },
            // Mission 2
            new string[] {
                "Sickles"
            },
            // Mission 3
            new string[] {
                "66 Slum Avenue",
                "Bullseye",
                "Love Planet",
                "13th Avenue",
                "Ice Age"
            },
            // Mission 4
            new string[] {
                "Chamber of Echoes",
                "Entranceway",
                "Living Statue Room",
                "Silence Statuary",
                "Incandescent Space",
                "Chamber of Echoes",
                "Endless Infernum",
                "Chamber of Sins",
                "Endless Infernum",
                "Giantwalker Chamber",
                "Cuntipede"
            },
            // Mission 5
            new string[] {
                "Giantwalker Chamber",
                "Demon Clown Chamber",
                "Giantwalker Chamber",
                "Endless Infernum",
                "Chamber of Echoes",
                "Entranceway",
                "Living Statue Room",
                "Entranceway",
                "Chamber of Echoes",
                "Endless Infernum",
                "Surge of Fortunas",
                "Azure Garden",
                "Backstabbers"
            },
            // Mission 6
            new string[] {
                "Mute Goddess' Chamber",
                "Chamber of 3 Trials",
                "Trial of Skill",
                "Chamber of 3 Trials",
                "Trial of Wisdom",
                "Chamber of 3 Trials",
                "Mute Goddess' Chamber",
                "Trial of Error"
            },
            // Mission 7
            new string[] {
                "The Dark Corridor",
                "Heavenrise Chamber",
                "The Divine Library",
                "Heavenrise Chamber",
                "Pitch-black Void",
                "Skull Spire",
                "Tranquil Souls Room",
                "Lift Room",
                "Chamber of Echoes",
                "Entranceway",
                "Cursed Skull Chamber",
                "Entranceway",
                "Chamber of Echoes",
                "Lift Room",
                "Tranquil Souls Room",
                "Skull Spire",
                "Moonlight Mile",
                "Peak of Darkness",
                "Foolishness"
            },
            // Mission 8
            new string[] {
                "Leviathan's Stomach",
                "Leviathan's Intestines",
                "Leviathan's Heartcore",
                "Leviathan's Intestines",
                "Leviathan's Intestines",
                "Leviathan's Retina",
                "Leviathan's Intestines",
                "Leviathan's Intestines",
                "Leviathan's Stomach",
                "Leviathan's Intestines",
                "Belly of the Beast"
            },
            // Mission 9
            new string[] {
                "The Rotating Bridge",
                "Provisions Storeroom",
                "Subterranean Garden",
                "Subground Water Vein",
                "Rounded Pathway",
                "Subterranean Lake",
                "Rounded Pathway",
                "Provisions Storeroom",
                "Rounded Pathway",
                "Subterranean Lake",
                "Limestone Cavern",
                "Heavy Metal Cave"
            },
            // Mission 10
            new string[] {
                "Limestone Cavern",
                "Sunken Opera House",
                "Limestone Cavern",
                "Subterranean Lake",
                "Rounded Pathway",
                "Subground Water Vein",
                "Subterranean Garden",
                "Provisions Storeroom",
                "The Rotating Bridge",
                "Dante Must Deliver"
            },
            // Mission 11
            new string[] {
                "Gears of Madness",
                "Altar of Evil Pathway",
                "Altar of Evil",
                "Temperance Wagon",
                "Temperance Wagon",
                "Temperance Wagon",
                "Berserk"
            },
            // Mission 12
            new string[] {
                "Torture Chamber",
                "Temperance Wagon",
                "Temperance Wagon",
                "Temperance Wagon",
                "Altar of Evil",
                "Altar of Evil Pathway",
                "Gears of Madness",
                "Marble Throughway",
                "The Rotating Bridge",
                "Spiral Corridor",
                "Timestomp"
            },
            // Mission 13
            new string[] {
                "Effervescence Corridor",
                "Spiral Staircase",
                "Lux-luminous Corridor",
                "Vestibule",
                "Lux-luminous Corridor",
                "Obsidian Path",
                "The Runback"
            },
            // Mission 14
            new string[] {
                "Lair of Judgement Ruins",
                "Underwater Elevator",
                "Subterran Lake",
                "Top Obsidian Path",
                "Vestibule",
                "Altar of Evil",
                "Temperance Wagon",
                "Temperance Wagon",
                "Temperance Wagon",
                "Hell's Highway",
                "Subterran Garden",
                "Subground Water Vein",
                "Love Planet",
                "Killer Bee"
            },
            // Mission 15
            new string[] {
                "Upper Subterran Garden",
                "Provisions Storeroom",
                "Devilsprout Lift",
                "Forbidden Land Front",
                "Devilsprout Lift",
                "Provisions Storeroom",
                "Top Subterria Lack",
                "Rounded Pathway",
                "Top Subterria Lack",
                "Rounded Pathway",
                "Provisions Storeroom",
                "Rounded Pathway",
                "Gears of Madness",
                "Altar of Evil Pathway",
                "Gears of Madness",
                "Provisions Storeroom",
                "The Quest for Aja"
            },
            // Mission 16
            new string[] {
                "Sun & Moon Chamber",
                "Entranceway",
                "Chamber of Sins",
                "Entranceway",
                "Sun & Moon Chamber",
                "Entranceway",
                "Living Statue Room",
                "Waking Sun Chamber",
                "Living Statue Room",
                "Entranceway",
                "Sun & Moon Chamber",
                "Ice Guardian's Chamber",
                "Surge of Fortunas",
                "Endless Infernum",
                "Giantwalker Chamber",
                "Incandescent Space",
                "The Divine Library",
                "Pretty Woman"
            },
            // Mission 17
            new string[] {
                "The Dark Corridor",
                "God-cube Chamber",
                "Tri-sealed Antechamber",
                "Trial of Wisdom",
                "Trial of Skill",
                "Pitch-black Void",
                "Skull Spire",
                "Moonlight Mile",
                "Dark-pact Chamber",
                "Apparition Incarnate",
                "Rise and Shine"
            },
            // Mission 18
            new string[] {
                "Unsacred Hellgate",
                "Damned Chess Board",
                "Road to Despair",
                "Lost Souls Nirvana",
                "Ice Guardian Reborn",
                "Lost Souls Nirvana",
                "Firestorm Reborn",
                "Lost Souls Nirvana",
                "Lightbeast Reborn",
                "Wily Stages"
            },
            // Mission 19
            new string[] {
                "Room of Fallen Ones",
                "Nirvana of Illusions",
                "Infinity Nirvana",
                "Lost Souls Nirvana",
                "Room of Fallen Ones",
                "Nirvana of Illusions",
                "End of the Line",
                "Jelly"
            },
            // Mission 20
            new string[] {
                "Wanna Know the Name?"
            }
        };
    }
}
