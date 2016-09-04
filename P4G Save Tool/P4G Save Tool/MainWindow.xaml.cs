using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace P4G_Save_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Persona
    {

        public string Name { get { return Database.personae[id]; } }
        public bool exists;
        public byte unknown0;
        public ushort id;
        public byte level { get; set; }
        public uint totalxp { get; set; }
        public ushort unknown1;
        public byte unknown2;
        public Item skill1 { get; set; }
        public Item skill2 { get; set; }
        public Item skill3 { get; set; }
        public Item skill4 { get; set; }
        public Item skill5 { get; set; }
        public Item skill6 { get; set; }
        public Item skill7 { get; set; }
        public Item skill8 { get; set; }
        public byte st { get; set; }
        public byte ma { get; set; }
        public byte de { get; set; }
        public byte ag { get; set; }
        public byte lu { get; set; }
        private Item FromID(ushort id)
        {
            if (id >= Database.allSkills.Length) return Database.skills[0];
            for (int i = 0; i < Database.skills.Count; i++)
                if (Database.skills[i].ID == id)
                    return Database.skills[i];
            return Database.skills[0];
        }
        public Persona(bool exists = true, byte unknown0 = 0, ushort id = 0, byte level = 0, uint totalxp = 0, ushort unknown1 = 0, byte unknown2 = 2, ushort skill1 = 0, ushort skill2 = 0, ushort skill3 = 0, ushort skill4 = 0, ushort skill5 = 0, ushort skill6 = 0, ushort skill7 = 0, ushort skill8 = 0, byte st = 0, byte ma = 0, byte de = 0, byte ag = 0, byte lu = 0)
        {
            this.exists = exists;
            this.unknown0 = unknown0;
            this.id = id;
            this.level = level;
            this.totalxp = totalxp;
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.skill1 = FromID(skill1);
            this.skill2 = FromID(skill2);
            this.skill3 = FromID(skill3);
            this.skill4 = FromID(skill4);
            this.skill5 = FromID(skill5);
            this.skill6 = FromID(skill6);
            this.skill7 = FromID(skill7);
            this.skill8 = FromID(skill8);
            this.st = st;
            this.ma = ma;
            this.de = de;
            this.ag = ag;
            this.lu = lu;
        }
    }
    public static class Utils
    {
        public static void Refresh(this UIElement uiElement)
        {
            Action EmptyDelegate = delegate () { };
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        public static Persona ReadPersona(this BinaryReader r)
        {
            bool exists = r.ReadBoolean();
            byte unknown0 = r.ReadByte();
            ushort id = r.ReadUInt16();
            byte level = r.ReadByte();

            r.BaseStream.Position += 3;
            uint totalxp = r.ReadUInt32();
            ushort unknown1 = 0;// r.ReadUInt16();
            byte unknown2 = 0;// r.ReadByte();
            ushort s1 = r.ReadUInt16();
            ushort s2 = r.ReadUInt16();
            ushort s3 = r.ReadUInt16();
            ushort s4 = r.ReadUInt16();
            ushort s5 = r.ReadUInt16();
            ushort s6 = r.ReadUInt16();
            ushort s7 = r.ReadUInt16();
            ushort s8 = r.ReadUInt16();
            byte st = r.ReadByte();
            byte ma = r.ReadByte();
            byte de = r.ReadByte();
            byte ag = r.ReadByte();
            byte lu = r.ReadByte();
            Persona p = new Persona(exists, unknown0, id, level, totalxp, unknown1, unknown2, s1, s2, s3, s4, s5, s6, s7, s8, st, ma, de, ag, lu);
            return p;
        }
        public static void WritePersona(this BinaryWriter w, Persona persona)
        {
            w.Write(persona.exists);
            w.Write(persona.unknown0);
            w.Write(persona.id);
            w.Write(persona.level);
            w.BaseStream.Position += 3;
            w.Write(persona.totalxp);
            //w.Write(persona.unknown1);
            //w.Write(persona.unknown2);
            w.Write(persona.skill1.ID);
            w.Write(persona.skill2.ID);
            w.Write(persona.skill3.ID);
            w.Write(persona.skill4.ID);
            w.Write(persona.skill5.ID);
            w.Write(persona.skill6.ID);
            w.Write(persona.skill7.ID);
            w.Write(persona.skill8.ID);
            w.Write(persona.st);
            w.Write(persona.ma);
            w.Write(persona.de);
            w.Write(persona.ag);
            w.Write(persona.lu);
        }
        public static string ReadJString(this BinaryReader r)
        {
            string str = "";
            for (int i = 0; i < 9; i++)
            {
                ushort charValue = r.ReadUInt16();
                byte[] utfValue = new byte[2] { (byte)(((charValue & 0xFF00) >> 8) - 0x60), 0 };
                str += Encoding.Unicode.GetString(utfValue).TrimEnd(new char[] { ' ', (char)160 });
            }
            return str;
        }

        public static string ReadPString(this BinaryReader r)
        {
            string b = "";
            List<byte> raw = new List<byte>();
            for (int i = 0; i < 9; i++)
            {
                ushort charValue = BitConverter.ToUInt16(BitConverter.GetBytes(r.ReadUInt16()).Reverse(), 0);
                if (charValue != 0)
                {
                    if (charValue > 33358 && charValue < 33402)
                        b += (char)((charValue - 33311));
                    else if (charValue > 33408 && charValue < 33435)
                        b += (char)((charValue - 33312));
                    else if (charValue > 33079)
                    {
                        b += (char)((charValue - 33047));
                    }
                }
                //else raw[i] = (byte)' ';
            }
            return b;// Encoding.UTF8.GetString(raw.ToArray()).TrimEnd(' ');
        }

        public static void WriteJString(this BinaryWriter w, string name)
        {
            const int stride = 2;
            byte[] rawString = Encoding.Unicode.GetBytes(name);
            for (int i = 0; i < 9; i++)
            {
                ushort charValue = 0;
                if (rawString.Length > (i * stride))
                {
                    if (rawString[i * stride] != 0xA0)
                    {
                        charValue = (ushort)((rawString[i * stride] + 0x60) << 8 | 0x80);
                        charValue = (ushort)((rawString[i * stride] + 0x60) << 8 | 0x80);
                    }
                }
                w.Write(charValue);
            }
        }

        public static void WritePString(this BinaryWriter w, string name)
        {
            byte[] raw = Encoding.Unicode.GetBytes(name);
            ushort charValue = 0;
            for(int i = 0; i < 9; i++)
            { 
                charValue = 0;
                if (i < name.Length)
                {
                    if (name[i] >= '0' && name[i] <= 'Z')
                        charValue = (ushort)(33311 + name[i]);
                    else if (name[i] >= 'a' && name[i] <= 'z')
                        charValue = (ushort)(33312 + name[i]);
                    else if (name[i] >= '!')
                        charValue = (ushort)(33047 + name[i]);
                }
                w.Write(BitConverter.ToUInt16(BitConverter.GetBytes(charValue).Reverse(), 0));
            }
        }

        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static void WriteUInt16BE(this BinaryWriter binRdr, ushort u16)
        {
            var v = BitConverter.GetBytes(u16);
            binRdr.Write(BitConverter.ToUInt16(v.Reverse(), 0));
        }

        public static Int16 ReadInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }
        public static Item[] GetFromDatabase(string[] database, ushort start, ushort length)
        {
            Item[] res = new Item[length];
            for(ushort i = 0; i < length; i++)
            {
                ushort id = (ushort)(i + start);
                res[i] = new Item(database[id], id);
            }
            return res;
        }

        public static Persona[] GetPersonaFromDatabase(ushort start, ushort length)
        {
            Persona[] res = new Persona[length];
            for (ushort i = 0; i < length; i++)
            {
                ushort id = (ushort)(i + start);
                res[i] = new Persona(true, 0, id, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1);
            }
            return res;
        }

        public static Visual GetDescendantByType(this Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

    }
    public class Item
    {
        private string name;
        private ushort id;
        public string Name { get { return name; } }
        public ushort ID { get { return id; } }
        public Item(string name, ushort id)
        {
            this.name = name;
            this.id = id;
        }
    };
    public class ItemStack
    {
        public string Text { get; set; }
        public ushort ID;
        public ItemStack(string text, ushort id)
        {
            Text = text;
            ID = id;
        }
    };
    public class ItemByte
    {
        private string name;
        private byte id;
        public string Name { get { return name; } }
        public byte ID { get { return id; } }
        public ItemByte(string name, byte id)
        {
            this.name = name;
            this.id = id;
        }
    };
    public class SocialLink
    {
        private string name;
        private byte id;
        private byte tarot;
        private byte level;
        private byte progress;
        public string Name { get { return name; } }
        public byte ID { get { return id; } }
        public byte Progress { get { return progress; } set { progress = value; } }
        public string Tarot { get { return Database.Arcana[tarot]; } }
        public byte Level { get { return level; } set { level = value; } }
        public SocialLink(string name, byte id, byte tarot, byte level, byte progress = 0)
        {
            this.name = name;
            this.id = id;
            this.tarot = tarot;
            this.level = level;
            this.progress = progress;
        }
        public SocialLink Copy()
        {
            return new SocialLink(name, id, tarot, level);
        }
        public SocialLink Copy(byte level, byte progress)
        {
            return new SocialLink(name, id, tarot, level, progress);
        }
    };
    public partial class MainWindow : MetroWindow
    {
        string filename;
        byte[] currentFileCopy;
        bool readyEvents;
        bool[] scroll = { false, false };
        int[] itemSel = new int[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        ScrollViewer invScroll, stackScroll;
        Item[] charEList;
        List<Item> armor;
        List<Item> accessories;
        List<Item> consumables;
        List<Item> books;
        List<Item> materials;
        List<Item> cards;
        List<Item> veggies;
        List<Item> minerals;
        List<Item> socialLink;
        List<Item> shelf;
        List<Item> costumes;
        List<Item> other;
        SocialLink[] socialLinkIDs;
        bool isFTP;
        string originalTitle = "P4G Save Tool";
        List<Item> personae;
        string[] courageLevels;
        string[] diligenceLevels;
        string[] understandingLevels;
        string[] expressionLevels;
        string[] knowledgeLevels;

        uint mcTotalXp;
        ushort[] socialStats;
        ushort[] equippedWeapons;
        ushort[] equippedArmors;
        ushort[] equippedAccessories;
        ushort[] equippedCostumes;
        public byte Day { get; set; }
        public byte NextDay { get; set; }
        public byte DayPhase { get; set; }
        public byte NextDayPhase { get; set; }
        byte mcLevel;
        uint yen;
        string firstname, surname;
        Persona[][] slots;
        List<Item>[] weps;
        List<Item>[] items;
        Persona[] compendium;

        public string Yen
        {
            get { return yen.ToString(); }
        } 

        public MainWindow()
        {
            isFTP = false;
            readyEvents = false;
            InitializeComponent();
            invScroll = (ScrollViewer)invBox.GetDescendantByType(typeof(ScrollViewer));
            stackScroll = (ScrollViewer)stackBox.GetDescendantByType(typeof(ScrollViewer));

            mcTotalXp = 0;

            compendium = new Persona[232];

            socialStats = new ushort[5];

            understandingLevels = new string[]{ "Basic", "Kindly", "Generous", "Motherly", "Saintly" };
            knowledgeLevels = new string[] { "Aware", "Informed", "Expert", "Professor", "Sage" };
            courageLevels = new string[] { "Average", "Reliable", "Brave", "Daring", "Heroic" };
            expressionLevels = new string[] { "Rough", "Eloquent", "Persuasive", "Touching", "Enthralling" };
            diligenceLevels = new string[] { "Callow", "Persistent", "Strong", "Persuasive", "Rock Solid" };

            ItemByte[] members = new ItemByte[7]
            {
                new ItemByte("Blank", 0),
                new ItemByte("Yosuke", 2),
                new ItemByte("Chie", 3),
                new ItemByte("Yukiko", 4),
                new ItemByte("Kanji", 6),
                new ItemByte("Naoto", 7),
                new ItemByte("Teddie", 8),
            };

            member1.ItemsSource = members;
            member2.ItemsSource = members;
            member3.ItemsSource = members;
            member1.SelectedIndex = 0;
            member2.SelectedIndex = 0;
            member3.SelectedIndex = 0;

            charEList = new Item[]
            {
                new Item("Yu Narukami", 0),
                new Item("Yosuke Hanamura", 1),
                new Item("Chie Satonaka", 2),
                new Item("Yukiko Amagi", 3),
                new Item("Kanji Tatsumi", 5),
                new Item("Naoto Shirogane", 6),
                new Item("Teddie", 7)
            };

            charBox.ItemsSource = charEList;
            charBox.SelectedIndex = 0;

            equippedWeapons = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            equippedArmors = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            equippedAccessories = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            equippedCostumes = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            invScroll.ScrollChanged += (object o, ScrollChangedEventArgs e) =>
            {
                if (invScroll.IsMouseOver)
                {
                    stackScroll.ScrollToVerticalOffset(invScroll.VerticalOffset);
                }
            };
            stackScroll.ScrollChanged += (object o, ScrollChangedEventArgs e) =>
            {
                if (stackScroll.IsMouseOver)
                {
                    invScroll.ScrollToVerticalOffset(stackScroll.VerticalOffset);
                }
            };

            string[] itmSections = new string[]
            {
                "Weapons",
                "Armor",
                "Accessories",
                "Consumables",
                "Materials",
                "Skill Cards",
                "Books",
                "Veggies",
                "Minerals",
                "Social Link",
                "Shelf",
                "Costumes",
                "Bugs",
                "Other"
            };

            socialLinkIDs = new SocialLink[]
            {
                new SocialLink("Blank", 0, 0, 0),
                new SocialLink("Investigation Team", 1, 1, 1),
                new SocialLink("Yosuke", 7, 2, 1),
                new SocialLink("Chie", 11, 8, 1),
                new SocialLink("Yukiko", 5, 3, 1),
                new SocialLink("Kanji", 10, 5, 1),
                new SocialLink("Teddie", 24, 18, 1),
                new SocialLink("Rise", 3, 7, 1),
                new SocialLink("Naoto", 14, 11, 1),
                new SocialLink("Fellow Athletes (Kou)", 16, 12, 1),
                new SocialLink("Fellow Athletes (Daisuke)", 17, 12, 1),
                new SocialLink("Ayane", 25, 20, 1),
                new SocialLink("Yumi", 27, 20, 1),
                new SocialLink("Naoki", 18, 13, 1),
                new SocialLink("Ai", 21, 19, 1),
                new SocialLink("Ai2", 22, 19, 1),
                new SocialLink("Nanako", 2, 9, 1),
                new SocialLink("Dojima", 8, 6, 1),
                new SocialLink("Adachi", 31, 25, 1),
                new SocialLink("Adachi (Hunger)", 32, 26, 1),
                new SocialLink("Fox", 13, 10, 1),
                new SocialLink("Hisano", 19, 14, 1),
                new SocialLink("Eri", 29, 15, 1),
                new SocialLink("Sayoko", 9, 16, 1),
                new SocialLink("Shu", 23, 17, 1),
                new SocialLink("Margaret", 20, 4, 1),
                new SocialLink("Marie", 33, 22, 1),
                new SocialLink("Marie2", 34, 22, 1),
                new SocialLink("The Seekers of Truth", 30, 21, 1)
            };

            inventory = new byte[2559];

            sLinkComboBox.ItemsSource = socialLinkIDs;
            sLinkComboBox.SelectedIndex = 0;

            personae = new List<Item>(Utils.GetFromDatabase(Database.personae, 1, 42));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 44, 8));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 53, 127));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 182, 32));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 224, 26));

            personae.Insert(0, new Item("Blank", 0));


            Database.skills = new List<Item>(Utils.GetFromDatabase(Database.allSkills, 0, 255));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 259, 42));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 349, 46));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 440, 13));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 472, 151));

            compendiumComboBox.ItemsSource = personae;
            compendiumComboBox.SelectedIndex = 0;

            Item wepBlank = Utils.GetFromDatabase(Database.allItems, 0, 1)[0];
            Item armorBlank = Utils.GetFromDatabase(Database.allItems, 256, 1)[0];
            Item accessoryBlank = Utils.GetFromDatabase(Database.allItems, 512, 1)[0];
            Item consumableBlank = Utils.GetFromDatabase(Database.allItems, 768, 1)[0];
            Item materialBlank = Utils.GetFromDatabase(Database.allItems, 1280, 1)[0];
            Item cardBlank = Utils.GetFromDatabase(Database.allItems, 1536, 1)[0];
            Item costumeBlank = Utils.GetFromDatabase(Database.allItems, 1792, 1)[0];
            Item otherBlank = Utils.GetFromDatabase(Database.allItems, 1024, 1)[0];
            Item mineralBlank = Utils.GetFromDatabase(Database.allItems, 2048, 1)[0];

            List<Item> mcWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1, 36));
            List<Item> yoWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 39, 32));
            List<Item> yuWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 77, 28));
            List<Item> chWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 112, 31));
            List<Item> kaWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 150, 26));
            List<Item> naWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 183, 15));
            List<Item> teWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 217, 22));

            List<Item> bugs = new List<Item>(Utils.GetFromDatabase(Database.allItems, 909, 7));

            armor = new List<Item>(Utils.GetFromDatabase(Database.allItems, 257, 8));
            accessories = new List<Item>(Utils.GetFromDatabase(Database.allItems, 513, 96));
            consumables = new List<Item>(Utils.GetFromDatabase(Database.allItems, 769, 51));
            books = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1136, 5));
            materials = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1281, 128));
            cards = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1537, 252));
            veggies = new List<Item>(Utils.GetFromDatabase(Database.allItems, 2089, 16));
            minerals = new List<Item>(Utils.GetFromDatabase(Database.allItems, 2128, 29));

            socialLink = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1184, 20));
            shelf = new List<Item>(Utils.GetFromDatabase(Database.allItems, 2056, 5));
            costumes = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1792, 193));
            other = new List<Item>();

            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 615, 69));
            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 685, 1));
            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 687, 8));

            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 754, 13));

            veggies.AddRange(Utils.GetFromDatabase(Database.allItems, 2107, 4));

            costumes.AddRange(Utils.GetFromDatabase(Database.allItems, 2040, 6));



            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 266, 6));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 287, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 293, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 307, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 315, 3));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 328, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 334, 5));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 347, 2));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 350, 9));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 367, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 372, 7));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 387, 6));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 394, 5));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 407, 6));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 414, 5));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 420, 13));

            shelf.AddRange(Utils.GetFromDatabase(Database.allItems, 1234, 13));

            books.AddRange(Utils.GetFromDatabase(Database.allItems, 1145, 7));
            books.AddRange(Utils.GetFromDatabase(Database.allItems, 1259, 20));

            socialLink.AddRange(Utils.GetFromDatabase(Database.allItems, 1207, 3));
            socialLink.AddRange(Utils.GetFromDatabase(Database.allItems, 1224, 3));
            socialLink.AddRange(Utils.GetFromDatabase(Database.allItems, 1228, 1));

            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1410, 21));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1432, 21));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1454, 20));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1475, 19));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1495, 19));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1513, 21));

            mcWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2305, 11));
            yoWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2326, 9));
            yuWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2345, 11));
            chWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2367, 9));
            kaWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2385, 3)); kaWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2389, 8));
            naWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2407, 8));
            teWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2425, 8));


            mcWeps.Add(new Item(Database.allItems[2434], 2434));
            yoWeps.Add(new Item(Database.allItems[2435], 2435));
            yuWeps.Add(new Item(Database.allItems[2437], 2437));
            chWeps.Add(new Item(Database.allItems[2436], 2436));
            kaWeps.Add(new Item(Database.allItems[2438], 2438));
            naWeps.Add(new Item(Database.allItems[2439], 2439));
            teWeps.Add(new Item(Database.allItems[2440], 2440));

            Database.weapons = new List<Item>();
            Database.weapons.AddRange(mcWeps);
            Database.weapons.AddRange(yoWeps);
            Database.weapons.AddRange(chWeps);
            Database.weapons.AddRange(yuWeps);
            Database.weapons.AddRange(kaWeps);
            Database.weapons.AddRange(naWeps);
            Database.weapons.AddRange(teWeps);

            mcWeps.Insert(0, wepBlank);
            yoWeps.Insert(0, wepBlank);
            yuWeps.Insert(0, wepBlank);
            chWeps.Insert(0, wepBlank);
            kaWeps.Insert(0, wepBlank);
            naWeps.Insert(0, wepBlank);
            teWeps.Insert(0, wepBlank);

            consumables.Insert(00, consumableBlank);
            armor.Insert(0, armorBlank);
            accessories.Insert(0, accessoryBlank);
            cards.Insert(0, cardBlank);

            minerals.Insert(0, mineralBlank);
            materials.Insert(0, materialBlank);
            veggies.Insert(0, consumableBlank);
            shelf.Insert(0, otherBlank);
            socialLink.Insert(0, otherBlank);
            books.Insert(0, otherBlank);

            other.Insert(0, otherBlank);
            bugs.Insert(0, otherBlank);

            Database.weapons.Insert(0, wepBlank);



            weps = new List<Item>[] { mcWeps, yoWeps, chWeps, yuWeps, null, kaWeps, naWeps, teWeps };
            items = new List<Item>[] { Database.weapons, armor, accessories, consumables, materials, cards, books, veggies, minerals, socialLink, shelf, costumes, bugs, other };

            itemSectBox.ItemsSource = itmSections;
            itemSectBox.SelectedIndex = 0;
            member.ItemsSource = Database.party;
            member.SelectedIndex = 0;

            skillBox1.ItemsSource = Database.skills;    compSkillBox1.ItemsSource = Database.skills;
            skillBox2.ItemsSource = Database.skills;    compSkillBox2.ItemsSource = Database.skills;
            skillBox3.ItemsSource = Database.skills;    compSkillBox3.ItemsSource = Database.skills;
            skillBox4.ItemsSource = Database.skills;    compSkillBox4.ItemsSource = Database.skills;
            skillBox5.ItemsSource = Database.skills;    compSkillBox5.ItemsSource = Database.skills;
            skillBox6.ItemsSource = Database.skills;    compSkillBox6.ItemsSource = Database.skills;
            skillBox7.ItemsSource = Database.skills;    compSkillBox7.ItemsSource = Database.skills;
            skillBox8.ItemsSource = Database.skills;    compSkillBox8.ItemsSource = Database.skills;


            itemBox.ItemsSource = items[0];
            personaIDs.ItemsSource = personae;
            phaseBox.ItemsSource = Database.phases;
            nextPhaseBox.ItemsSource = Database.phases;
            skillBox1.SelectedIndex = 0;    compSkillBox1.SelectedIndex = 0;
            skillBox2.SelectedIndex = 0;    compSkillBox2.SelectedIndex = 0;
            skillBox3.SelectedIndex = 0;    compSkillBox3.SelectedIndex = 0;
            skillBox4.SelectedIndex = 0;    compSkillBox4.SelectedIndex = 0;
            skillBox5.SelectedIndex = 0;    compSkillBox5.SelectedIndex = 0;
            skillBox6.SelectedIndex = 0;    compSkillBox6.SelectedIndex = 0;
            skillBox7.SelectedIndex = 0;    compSkillBox7.SelectedIndex = 0;
            skillBox8.SelectedIndex = 0;    compSkillBox8.SelectedIndex = 0;
            personaIDs.SelectedIndex = 0;
            phaseBox.SelectedIndex = 0;
            nextPhaseBox.SelectedIndex = 0;
            itemBox.SelectedIndex = 0;
            slots = new Persona[][] 
            {
               new Persona[12] { new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() }
            };

            Day = 0;
            NextDay = 0;

            courageBox.ItemsSource = courageLevels;
            knowledgeBox.ItemsSource = knowledgeLevels;
            expressionBox.ItemsSource = expressionLevels;
            understandingBox.ItemsSource = understandingLevels;
            diligenceBox.ItemsSource = diligenceLevels;

            courageBox.SelectedIndex = 0;
            knowledgeBox.SelectedIndex = 0;
            expressionBox.SelectedIndex = 0;
            understandingBox.SelectedIndex = 0;
            diligenceBox.SelectedIndex = 0;

            wepBox.ItemsSource = weps[(charBox.SelectedItem as Item).ID];
            wepBox.InvalidateVisual();
            armBox.ItemsSource = armor;
            accBox.ItemsSource = accessories;
            cosBox.ItemsSource = costumes;
            wepBox.SelectedIndex = 0;
            armBox.SelectedIndex = 0;
            accBox.SelectedIndex = 0;
            cosBox.SelectedIndex = 0;

            dayBox.Text = "0";
            nextDayBox.Text = "0";

            string[] args = Environment.GetCommandLineArgs();
            if(args.Length > 1)
            {
                string file = args[1];
                if(File.Exists(file))
                {
                    int last = file.LastIndexOf('\\');
                    filename = file.Substring(last+1, file.Length-(last + 1));

                    OpenFile(File.OpenRead(file));
                    Title = originalTitle + " - " + filename;
                    isFTP = false;
                }
            }
        }

        private int PointsToStatusLevel(byte status, ushort points)
        {
            if (status == 3)
            {
                if (points <= 15)
                    return 1;
                else if (points >= 16 && points <= 39)
                    return 2;
                else if (points >= 40 && points <= 79)
                    return 3;
                else if (points >= 80 && points <= 139)
                    return 4;
                else if (points >= 140)
                    return 5;
            }
            if (status == 1)
            {
                if (points <= 29)
                    return 1;
                else if (points >= 30 && points <= 79)
                    return 2;
                else if (points >= 80 && points <= 149)
                    return 3;
                else if (points >= 150 && points <= 239)
                    return 4;
                else if (points >= 240)
                    return 5;
            }
            else if (status == 0)
            {
                if (points <= 15)
                    return 1;
                else if (points >= 16 && points <= 39)
                    return 2;
                else if (points >= 40 && points <= 79)
                    return 3;
                else if (points >= 80 && points <= 139)
                    return 4;
                else if (points >= 140)
                    return 5;
            }
            else if (status == 4)
            {
                if (points <= 12)
                    return 1;
                else if (points >= 13 && points <= 32)
                    return 2;
                else if (points >= 33 && points <= 52)
                    return 3;
                else if (points >= 53 && points <= 84)
                    return 4;
                else if (points >= 85)
                    return 5;
            }
            else if (status == 2)
            {
                if (points <= 15)
                    return 1;
                else if (points >= 16 && points <= 39)
                    return 2;
                else if (points >= 40 && points <= 79)
                    return 3;
                else if (points >= 80 && points <= 139)
                    return 4;
                else if (points >= 140)
                    return 5;
            }
            return 0;
        }

        private ushort StatusLevelToPoints(byte status, byte level)
        {
            if (status == 3)
            {
                if (level == 1)
                    return 15;
                else if (level == 2)
                    return 16;
                else if (level == 3)
                    return 40;
                else if (level == 4)
                    return 80;
                else if (level == 5)
                    return 140;
            }
            if (status == 1)
            {
                if (level == 1)
                    return 29;
                else if (level == 2)
                    return 30;
                else if (level == 3)
                    return 80;
                else if (level == 4)
                    return 150;
                else if (level == 5)
                    return 240;
            }
            else if (status == 0)
            {
                if (level == 1)
                    return 15;
                else if (level == 2)
                    return 16;
                else if (level == 3)
                    return 40;
                else if (level == 4)
                    return 80;
                else if (level == 5)
                    return 140;

            }
            else if (status == 4)
            {
                if (level == 1)
                    return 12;
                else if (level == 2)
                    return 13;
                else if (level == 3)
                    return 33;
                else if (level == 4)
                    return 53;
                else if (level == 5)
                    return 85;
            }
            else if (status == 2)
            {
                if (level == 1)
                    return 15;
                else if (level == 2)
                    return 16;
                else if (level == 3)
                    return 40;
                else if (level == 4)
                    return 80;
                else if (level == 5)
                    return 140;
            }
            return 0;
        }

        private void personaID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private byte[] inventory;

        private void personaIDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                Persona persona = slots[member.SelectedIndex][personaSlot.SelectedIndex];
                persona.id = (personaIDs.SelectedItem as Item).ID;
                if (persona.level == 0 && persona.id != 0 && persona.exists == false)
                    persona.level = 1;
                persona.exists = persona.id != 0;
            }
        }

        private void skillBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill1 = (skillBox1.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill2 = (skillBox2.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill3 = (skillBox3.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill4 = (skillBox4.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill5 = (skillBox5.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill6 = (skillBox6.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill7 = (skillBox7.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill8 = (skillBox8.SelectedItem as Item);
            }
        }

        private void compSkillBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                (compendiumBox.SelectedItem as Persona).skill1 = (compSkillBox1.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill2 = (compSkillBox2.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill3 = (compSkillBox3.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill4 = (compSkillBox4.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill5 = (compSkillBox5.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill6 = (compSkillBox6.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill7 = (compSkillBox7.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill8 = (compSkillBox8.SelectedItem as Item);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            readyEvents = true;
        }

        private void personaSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                readyEvents = false;
                for(int i = 0; i < personae.Count; i++)
                    if((personaIDs.Items[i] as Item).ID == slots[member.SelectedIndex][personaSlot.SelectedIndex].id)
                        personaIDs.SelectedIndex = i;
                skillBox1.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill1;
                skillBox2.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill2;
                skillBox3.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill3;
                skillBox4.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill4;
                skillBox5.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill5;
                skillBox6.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill6;
                skillBox7.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill7;
                skillBox8.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill8;

                LVSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].level;
                STSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].st;
                MASlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].ma;
                DESlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].de;
                AGSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].ag;
                LUSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].lu;

                xpBox.Text = slots[member.SelectedIndex][personaSlot.SelectedIndex].totalxp.ToString();

                readyEvents = true;
            }
        }

        private void LVSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
            {
                slots[member.SelectedIndex][personaSlot.SelectedIndex].level = (byte)e.NewValue;
                if (slots[member.SelectedIndex][personaSlot.SelectedIndex].level > 99)
                    LVVal.Foreground = new SolidColorBrush(Colors.Red);
                else
                    LVVal.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void STSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].st = (byte)e.NewValue;

        }

        private void MASlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].ma = (byte)e.NewValue;

        }

        private void DESlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].de = (byte)e.NewValue;

        }

        private void AGSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].ag = (byte)e.NewValue;

        }

        private void LUSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].lu = (byte)e.NewValue;

        }

        private void member_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            personaSlot.SelectedIndex = 0;
            if (member.SelectedIndex != 0)
                personaSlot.IsEnabled = false;
            else personaSlot.IsEnabled = true;
            personaSlot_SelectionChanged(null, null);
            MCLVSlider.IsEnabled = member.SelectedIndex == 0;
            mcXpBox.IsEnabled = member.SelectedIndex == 0;
            calcXp.IsEnabled = member.SelectedIndex == 0;

        }

        private void yenBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(readyEvents)
            {
                uint result = 0;
                if (uint.TryParse(yenBox.Text, out result))
                    yen = result;
                else
                    yenBox.Text = yen.ToString();

            }
        }

        private void phaseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(readyEvents)
            {
                DayPhase = (byte)phaseBox.SelectedIndex;
            }
        }

        private void fnBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                firstname = fnBox.Text;
                Database.party[0] = firstname + " " + surname;
                charEList[0] = new Item(firstname + " " + surname, charEList[0].ID);
                member.ItemsSource = Database.party;
                charBox.ItemsSource = charEList;
                member.SelectedIndex = 1;
                member.SelectedIndex = 0;
                charBox.SelectedIndex = 1;
                charBox.SelectedIndex = 0;

            }
        }

        private void snBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                surname = snBox.Text;
                Database.party[0] = firstname + " " + surname;
                charEList[0] = new Item(firstname + " " + surname, charEList[0].ID);
                member.ItemsSource = Database.party;
                charBox.ItemsSource = charEList;
                member.SelectedIndex = 1;
                member.SelectedIndex = 0;
                charBox.SelectedIndex = 1;
                charBox.SelectedIndex = 0;

            }
        }

        private void SaveFile(Stream stream)
        {
            using (BinaryWriter w = new BinaryWriter(stream))
            {
                w.Write(currentFileCopy);
                w.BaseStream.Position = 16;
                w.WriteJString(surname);
                w.BaseStream.Position = 34;
                w.WriteJString(firstname);
                w.BaseStream.Position = 88;
                w.Write(yen);
                for (byte i = 0; i < 3; i++)
                {
                    w.Write(((partymembers.Children[i] as ComboBox).SelectedItem as ItemByte).ID);
                    w.BaseStream.Position++;
                }
                w.BaseStream.Position = 100;
                w.WritePString(surname);
                w.WritePString(firstname);
                w.BaseStream.Position = 136;
                for(int i = 0; i < invBox.Items.Count; i++)
                {
                    inventory[(invBox.Items[i] as Item).ID] = byte.Parse((stackBox.Items[i] as TextBox).Text);
                }
                w.Write(inventory);
                w.BaseStream.Position = 2700;
                for (int i = 0; i < 12; i++)
                {
                    w.WritePersona(slots[0][i]);
                    w.BaseStream.Position += 15;
                }
                w.BaseStream.Position = 3360;
                w.Write(equippedWeapons[0]);
                w.Write(equippedArmors[0]);
                w.Write(equippedAccessories[0]);
                w.Write(equippedCostumes[0]);
                w.BaseStream.Position = 3492;
                //              w.BaseStream.Position = 3500;
                for (int i = 1; i < 8; i++)
                {
                    w.Write(equippedWeapons[i]);
                    w.Write(equippedArmors[i]);
                    w.Write(equippedAccessories[i]);
                    w.Write(equippedCostumes[i]);
                    w.WritePersona(slots[i][0]);
                    w.BaseStream.Position += 91;
                }

                w.BaseStream.Position = 3290;
                w.Write(mcLevel);
                w.BaseStream.Position = 3292;
                w.Write(mcTotalXp);

                w.BaseStream.Position = 3336;
                for(byte i = 0; i < 5; i++)
                    w.Write(socialStats[i]);

                w.BaseStream.Position = 6484;
                w.Write(Day);
                w.BaseStream.Position++;
                w.Write(DayPhase);
                w.BaseStream.Position = 6492;
                w.Write(NextDay);
                w.BaseStream.Position++;
                w.Write(NextDayPhase);
                w.BaseStream.Position = 6512;
                byte[] chunk = new byte[368];
                Array.Clear(chunk, 0, chunk.Length);
                w.Write(chunk);
                w.BaseStream.Position = 6512;
                byte lcount = (byte)(23 <= sLinkBox.Items.Count ? 23 : sLinkBox.Items.Count);
                for (byte i = 0; i < lcount; i++)
                {
                    SocialLink s = sLinkBox.Items[i] as SocialLink;
                    w.Write(s.ID);
                    w.BaseStream.Position++;
                    w.Write(s.Level);
                    w.BaseStream.Position++;
                    w.Write(s.Progress);
                    if (i < lcount - 1) w.BaseStream.Position += 11;
                }
                w.BaseStream.Position = 9688;
                byte lcount2 = (byte)(compendiumBox.Items.Count);
                byte[] zeromemory = new byte[11937];
                Array.Clear(zeromemory, 0, zeromemory.Length);
                w.Write(zeromemory);
                for (int i = 0; i < lcount2; i++)
                {
                    Persona persona = compendiumBox.Items[i] as Persona;
                    w.BaseStream.Position = 9688 + (48*(persona.id-1));
                    w.WritePersona(persona);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(File.OpenWrite(filename));
        }

        private void itemSectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(readyEvents)
            {
                itemBox.ItemsSource = items[itemSectBox.SelectedIndex];
                itemBox.SelectedIndex = itemSel[itemSectBox.SelectedIndex];
            }
        }

        private void AddInventoryItem(Item item, byte stack)
        {
            if (item.Name == "Blank" || item.ID == 1792) return;
            invBox.Items.Add(item);
            invBox.SelectedItem = item;
            var box = new TextBox() { Text = stack.ToString(), Width = 30, Height = 16, TextWrapping = TextWrapping.NoWrap, TextAlignment = TextAlignment.Right };
            box.PreviewTextInput += personaID_PreviewTextInput;
            box.SelectionChanged += (object o, RoutedEventArgs e) => 
            {
                stackBox.SelectedItem = e.Source;
            };
            box.TextChanged += itemStack_TextChanged;
            stackBox.Items.Add(box);
            stackBox.SelectedIndex = invBox.SelectedIndex;
        }

        private void AddCompendiumItem(Persona persona)
        {
            if (persona.Name == "Blank" || persona.id == 0) return;
            compendiumBox.Items.Add(persona);
        }

        private void RemoveInventoryItem(Item item)
        {
            int i = invBox.Items.IndexOf(item);
            invBox.Items.RemoveAt(i);
            stackBox.Items.RemoveAt(i);
        }

        private void itemBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                if (itemBox.SelectedIndex != -1)
                {
                    itemSel[itemSectBox.SelectedIndex] = itemBox.SelectedIndex;
                    if (invBox.Items.Count == 0)
                    {
                        AddInventoryItem(items[itemSectBox.SelectedIndex][itemSel[itemSectBox.SelectedIndex]], 1);
                    }
                    else
                        for (int i = 0; i < invBox.Items.Count; i++)
                            if ((invBox.Items[i] as Item).ID != items[itemSectBox.SelectedIndex][itemSel[itemSectBox.SelectedIndex]].ID)
                            {
                                if (i == invBox.Items.Count - 1)
                                {
                                    AddInventoryItem(items[itemSectBox.SelectedIndex][itemSel[itemSectBox.SelectedIndex]], 1);
                                    break;
                                }
                            }
                            else
                            {
                                invBox.SelectedIndex = i;
                                break;
                            }
                }
                else
                {
                    itemSel[itemSectBox.SelectedIndex] = itemBox.SelectedIndex = 0;
                }
            }
        }

        private void invBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackBox.SelectedIndex = invBox.SelectedIndex;
        }

        private void stackBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            invBox.SelectedIndex = stackBox.SelectedIndex;
        }

        private void itemStack_TextChanged(object sender, TextChangedEventArgs e)
        {
            stackBox.SelectedIndex = stackBox.Items.IndexOf(e.Source);
            byte result = 0;
            if (!byte.TryParse((stackBox.SelectedItem as TextBox).Text, out result))
                (stackBox.SelectedItem as TextBox).Text = inventory[(invBox.Items[stackBox.SelectedIndex] as Item).ID].ToString();
            else
            {
                inventory[(invBox.Items[stackBox.SelectedIndex] as Item).ID] = result;
            }
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void skillBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (Item i in (e.Source as ComboBox).Items)
            {
                if (i.Name.ToString().ToUpper().Contains(e.Text.ToUpper()))
                {
                    (e.Source as ComboBox).SelectedItem = i;
                    break;
                }
            }
            e.Handled = true;
        }

        private void skillBox1_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            //if (e.Text.Length < 3)
            //    (e.Source as ComboBox).IsEditable = false;
            //else (e.Source as ComboBox).IsEditable = true;
        }

        private void MCLVSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(readyEvents)
            {
                mcLevel = (byte)(e.Source as Slider).Value;
                if (mcLevel > 99)
                    MCLVVal.Foreground = new SolidColorBrush(Colors.Red);
                else
                    MCLVVal.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            deleteMenuItem.IsEnabled = invBox.SelectedIndex != -1;
        }

        private void ContextMenu_Opened2(object sender, RoutedEventArgs e)
        {
            deleteMenuItem2.IsEnabled = sLinkBox.SelectedIndex != -1;
        }

        private void ContextMenu_Opened3(object sender, RoutedEventArgs e)
        {
            deleteMenuItem3.IsEnabled = compendiumBox.SelectedIndex != -1;
            deleteAllMenuItem.IsEnabled = compendiumBox.Items.Count > 0;
        }

        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (readyEvents)
            {
                RemoveInventoryItem(invBox.SelectedItem as Item);
            }
        }

        private void deleteMenuItem_Click2(object sender, RoutedEventArgs e)
        {
            if (readyEvents)
            {
                sLinkBox.Items.Remove(sLinkBox.SelectedItem);
            }
        }

        private void deleteMenuItem_Click3(object sender, RoutedEventArgs e)
        {
            if (readyEvents)
            {
                compendiumBox.Items.Remove(compendiumBox.SelectedItem);
            }
        }

        private void charBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                int sect = (charBox.SelectedItem as Item).ID;
                ushort[] eq = new ushort[8];
                equippedWeapons.CopyTo(eq, 0);
                wepBox.ItemsSource = weps[sect];
                for (int i = 0; i < weps[sect].Count; i++)
                {
                    if (weps[sect][i].ID == eq[sect])
                        wepBox.SelectedItem = weps[sect][i];
                    if (weps[sect][i].ID == 0)
                        wepBox.SelectedIndex = 0;
                }
                for (int i = 0; i < armor.Count; i++)
                {
                    if (armor[i].ID == equippedArmors[(charBox.SelectedItem as Item).ID])
                    {
                        armBox.SelectedIndex = 0;
                        armBox.SelectedItem = armor[i];
                    }
                    if (equippedArmors[(charBox.SelectedItem as Item).ID] == 0)
                        armBox.SelectedIndex = 0;
                }
                for (int i = 0; i < accessories.Count; i++)
                {
                    if (accessories[i].ID == equippedAccessories[(charBox.SelectedItem as Item).ID])
                    {
                        accBox.SelectedIndex = 0;
                        accBox.SelectedItem = accessories[i];
                    }
                    if (equippedAccessories[(charBox.SelectedItem as Item).ID] == 0)
                        accBox.SelectedIndex = 0;
                }
                for (int i = 0; i < costumes.Count; i++)
                {
                    if (costumes[i].ID == equippedCostumes[(charBox.SelectedItem as Item).ID])
                    {
                        cosBox.SelectedIndex = 0;
                        cosBox.SelectedItem = costumes[i];
                    }
                    if (equippedCostumes[(charBox.SelectedItem as Item).ID] == 0)
                        cosBox.SelectedIndex = 0;
                }
            }
        }

        private void wepBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(readyEvents)
                equippedWeapons[(charBox.SelectedItem as Item).ID] = wepBox.SelectedItem != null ? (wepBox.SelectedItem as Item).ID : (ushort)0;
        }

        private void armBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedArmors[charBox.SelectedIndex] = (armBox.SelectedItem as Item).ID;

        }

        private void accBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedAccessories[charBox.SelectedIndex] = (accBox.SelectedItem as Item).ID;

        }

        private void cosBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedCostumes[charBox.SelectedIndex] = (cosBox.SelectedItem as Item).ID;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void sLinkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                if (sLinkComboBox.SelectedIndex != 0)
                {
                    for (byte i = 0; i < sLinkBox.Items.Count; i++)
                        if ((sLinkBox.Items[i] as SocialLink).ID == (sLinkComboBox.SelectedItem as SocialLink).ID)
                            return;
                    sLinkBox.Items.Add(socialLinkIDs[sLinkComboBox.SelectedIndex].Copy());
                }
            }
        }

        private void OpenFile(Stream file)
        {
            using (BinaryReader r = new BinaryReader(file))
            {
                r.BaseStream.Position = 16;
                surname = r.ReadJString();
                r.BaseStream.Position = 34;
                firstname = r.ReadJString();
                r.BaseStream.Position = 88;
                yen = r.ReadUInt32();
                for (byte i = 0; i < 3; i++)
                {
                    byte c = r.ReadByte();
                    for (byte p = 0; p < 7; p++)
                        if (((partymembers.Children[i] as ComboBox).Items[p] as ItemByte).ID == c)
                            (partymembers.Children[i] as ComboBox).SelectedIndex = p;
                    r.BaseStream.Position++;
                }
                r.BaseStream.Position = 100;
                surname = r.ReadPString();
                firstname = r.ReadPString();
                r.BaseStream.Position = 136;
                inventory = r.ReadBytes(2559);
                r.BaseStream.Position = 2700;
                for (int i = 0; i < 12; i++)
                {
                    slots[0][i] = r.ReadPersona();
                    r.BaseStream.Position += 15;
                }
                r.BaseStream.Position = 3336;
                for (byte i = 0; i < 5; i++)
                    socialStats[i] = r.ReadUInt16();

                r.BaseStream.Position = 3360;
                equippedWeapons[0] = r.ReadUInt16();
                equippedArmors[0] = r.ReadUInt16();
                equippedAccessories[0] = r.ReadUInt16();
                equippedCostumes[0] = r.ReadUInt16();
                r.BaseStream.Position = 3492;
                //                  memoryReader.BaseStream.Position = 3500;
                for (int i = 1; i < 8; i++)
                {
                    equippedWeapons[i] = r.ReadUInt16();
                    equippedArmors[i] = r.ReadUInt16();
                    equippedAccessories[i] = r.ReadUInt16();
                    equippedCostumes[i] = r.ReadUInt16();
                    slots[i][0] = r.ReadPersona();
                    r.BaseStream.Position += 91;
                }

                r.BaseStream.Position = 3290;
                mcLevel = r.ReadByte();
                r.BaseStream.Position = 3292;
                mcTotalXp = r.ReadUInt32();

                r.BaseStream.Position = 6484;
                Day = r.ReadByte();
                r.BaseStream.Position++;
                DayPhase = r.ReadByte();
                r.BaseStream.Position = 6492;
                NextDay = r.ReadByte();
                r.BaseStream.Position++;
                NextDayPhase = r.ReadByte();
                r.BaseStream.Position = 6512;
                sLinkBox.Items.Clear();
                for (byte i = 0; i < 23; i++)
                {
                    byte id = r.ReadByte();
                    r.ReadByte();
                    byte level = r.ReadByte();
                    r.ReadByte();
                    byte progress = r.ReadByte();
                    if (id != 0)
                    {
                        for (byte q = 0; q < socialLinkIDs.Length; q++)
                        {
                            if (socialLinkIDs[q].ID == id)
                                sLinkBox.Items.Add(socialLinkIDs[q].Copy(level, progress));
                        }
                    }

                    if (i < 22) r.BaseStream.Position += 11;
                }
                compendiumBox.Items.Clear();
                //r.BaseStream.Position = 109620;
                r.BaseStream.Position = 9688;
                for (int i = 0; i < 249; i++)
                {
                    r.BaseStream.Position = 9688 + (48 * i);
                    Persona persona = r.ReadPersona();
                    AddCompendiumItem(persona);
                }


                r.BaseStream.Position = 0;
                currentFileCopy = r.ReadBytes((int)r.BaseStream.Length);

                if(compendiumBox.Items.Count > 0)
                    compendiumBox.SelectedIndex = 0;
                Database.party[0] = firstname + surname;
                member.ItemsSource = Database.party;
                member.InvalidateVisual();
                yenBox.Text = yen.ToString();
                phaseBox.SelectedIndex = DayPhase;
                nextPhaseBox.SelectedIndex = NextDayPhase;
                dayBox.Text = Day.ToString();
                nextDayBox.Text = NextDay.ToString();
                snBox.Text = surname;
                fnBox.Text = firstname;

                mcXpBox.Text = mcTotalXp.ToString();

                MCLVSlider.Value = mcLevel;

                readyEvents = false;
                courageBox.SelectedIndex = PointsToStatusLevel(0, socialStats[0]) - 1;
                knowledgeBox.SelectedIndex = PointsToStatusLevel(1, socialStats[1]) - 1;
                diligenceBox.SelectedIndex = PointsToStatusLevel(2, socialStats[2]) - 1;
                understandingBox.SelectedIndex = PointsToStatusLevel(3, socialStats[3]) - 1;
                expressionBox.SelectedIndex = PointsToStatusLevel(4, socialStats[4]) - 1;
                readyEvents = true;


                invBox.Items.Clear();
                stackBox.Items.Clear();

                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] != 0)
                    {
                        AddInventoryItem(new Item(Database.allItems[i], (ushort)i), inventory[i]);
                    }
                }
                Array.Clear(inventory, 0, inventory.Length);

                member.SelectedIndex = 1;
                member.SelectedIndex = 0;

                member_SelectionChanged(null, null);
                charBox_SelectionChanged(null, null);
            }
        }

        private void OpenFTP_Click(object sender, RoutedEventArgs e)
        {
            string ftpAddress = "";
            string Username = "";
            string Password = "";
            bool UseBinary = true;
            bool UsePassive = false;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpAddress);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.KeepAlive = true;
            request.UsePassive = UsePassive;
            request.UseBinary = UseBinary;

            request.Credentials = new NetworkCredential(Username, Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            OpenFile(responseStream);
            response.Close();
            filename = ftpAddress; 
            isFTP = true;
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Binary files (*.bin)|*.bin";
            if (d.ShowDialog() == true)
            {
                SaveFile(File.OpenWrite(filename = d.FileName));
                Title = originalTitle + " - " + d.SafeFileName;
                isFTP = false;
            }
        }

        private void compendiumBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (compendiumBox.SelectedItem == null)
            {
                compSkillBox1.IsEnabled = false;
                compSkillBox2.IsEnabled = false;
                compSkillBox3.IsEnabled = false;
                compSkillBox4.IsEnabled = false;
                compSkillBox5.IsEnabled = false;
                compSkillBox6.IsEnabled = false;
                compSkillBox7.IsEnabled = false;
                compSkillBox8.IsEnabled = false;
                readyEvents = false;
                compSkillBox1.SelectedItem = Database.skills[0];
                compSkillBox2.SelectedItem = Database.skills[0];
                compSkillBox3.SelectedItem = Database.skills[0];
                compSkillBox4.SelectedItem = Database.skills[0];
                compSkillBox5.SelectedItem = Database.skills[0];
                compSkillBox6.SelectedItem = Database.skills[0];
                compSkillBox7.SelectedItem = Database.skills[0];
                compSkillBox8.SelectedItem = Database.skills[0];
                readyEvents = true;
                compendiumComboBox.SelectedIndex = 0;

            }
            else
            {
                compSkillBox1.IsEnabled = true;
                compSkillBox2.IsEnabled = true;
                compSkillBox3.IsEnabled = true;
                compSkillBox4.IsEnabled = true;
                compSkillBox5.IsEnabled = true;
                compSkillBox6.IsEnabled = true;
                compSkillBox7.IsEnabled = true;
                compSkillBox8.IsEnabled = true;
                for (int i = 0; i < compendiumComboBox.Items.Count; i++)
                {
                    if ((compendiumComboBox.Items[i] as Item).ID == (compendiumBox.SelectedItem as Persona).id)
                        compendiumComboBox.SelectedIndex = i;
                }
                if (readyEvents)
                {
                    readyEvents = false;
                    compSkillBox1.SelectedItem = (compendiumBox.SelectedItem as Persona).skill1;
                    compSkillBox2.SelectedItem = (compendiumBox.SelectedItem as Persona).skill2;
                    compSkillBox3.SelectedItem = (compendiumBox.SelectedItem as Persona).skill3;
                    compSkillBox4.SelectedItem = (compendiumBox.SelectedItem as Persona).skill4;
                    compSkillBox5.SelectedItem = (compendiumBox.SelectedItem as Persona).skill5;
                    compSkillBox6.SelectedItem = (compendiumBox.SelectedItem as Persona).skill6;
                    compSkillBox7.SelectedItem = (compendiumBox.SelectedItem as Persona).skill7;
                    compSkillBox8.SelectedItem = (compendiumBox.SelectedItem as Persona).skill8;
                    readyEvents = true;
                }
            }
        }

        private void nextPhaseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                NextDayPhase = (byte)nextPhaseBox.SelectedIndex;
            }
        }

        private void compendiumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                bool exists = false;
                if (compendiumComboBox.SelectedIndex == 0)
                    compendiumBox.SelectedIndex = -1;
                else
                {
                    for (int i = 0; i < compendiumBox.Items.Count; i++)
                    {
                        if ((compendiumBox.Items[i] as Persona).id == (compendiumComboBox.SelectedItem as Item).ID)
                        {
                            exists = true;
                            compendiumBox.SelectedIndex = i;
                            compendiumBox.ScrollIntoView(compendiumBox.SelectedItem);
                        }
                    }
                    if(!exists)
                    {
                        AddCompendiumItem(new Persona(true, 0, (compendiumComboBox.SelectedItem as Item).ID, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1));
                        compendiumBox.SelectedIndex = compendiumBox.Items.Count-1;
                        compendiumBox.ScrollIntoView(compendiumBox.SelectedItem);
                    }
                }
            }
        }

        private void deleteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            compendiumBox.Items.Clear();
            compendiumBox.SelectedIndex = -1;
        }

        private void xpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(readyEvents)
            {
                uint result = 0;
                if (uint.TryParse(xpBox.Text, out result))
                    slots[member.SelectedIndex][personaSlot.SelectedIndex].totalxp = result;
                else
                xpBox.Text = slots[member.SelectedIndex][personaSlot.SelectedIndex].totalxp.ToString();
            }
        }

        private void nextDayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte result = 0;
            if (!byte.TryParse(nextDayBox.Text, out result))
            {
                int result2 = 0;
                if (int.TryParse(dayBox.Text, out result2))
                {
                    if (result2 > 255)
                        MessageBox.Show(this, "Max is 255.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (result2 < 0)
                        MessageBox.Show(this, "Day number can't be negative.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NextDay = result;
                    return;
                }
                MessageBox.Show(this, "Somehow you've managed to input something that's not a number, good job buddy.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else NextDay = result;
        }

        private void dayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte result = 0;
            if (!byte.TryParse(dayBox.Text, out result))
            {
                int result2 = 0;
                if(int.TryParse(dayBox.Text, out result2))
                {
                    if(result2 > 255)
                        MessageBox.Show(this, "Max is 255.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if(result2 < 0)
                        MessageBox.Show(this, "Day number can't be negative.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Day = result;
                    return;
                }
                MessageBox.Show(this, "Somehow you've managed to input something that's not a number, good job buddy.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else Day = result;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                OpenFile(File.OpenRead(filename = files[0]));
                Title = originalTitle + " - " + filename;
                isFTP = false;
            }
        }

        private void courageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[0] = StatusLevelToPoints(0, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void knowledgeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[1] = StatusLevelToPoints(1, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void expressionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[4] = StatusLevelToPoints(4, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void understandingBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[3] = StatusLevelToPoints(3, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void diligenceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[2] = StatusLevelToPoints(2, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void mcXpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                uint result = 0;
                if (uint.TryParse(mcXpBox.Text, out result))
                    mcTotalXp = result;
                else
                    mcXpBox.Text = mcTotalXp.ToString();
            }
        }

        private void calcXp_Click(object sender, RoutedEventArgs e)
        {
            byte level = mcLevel;
            uint xp = (uint)(((uint)Math.Pow(level, 4) + 4 * (uint)Math.Pow(level, 3) + 53 * (uint)Math.Pow(level, 2) - 58 * level)/10);
            mcXpBox.Text = xp.ToString();
        }

        private void calcXp_Copy_Click(object sender, RoutedEventArgs e)
        {
            byte level = slots[member.SelectedIndex][personaSlot.SelectedIndex].level;
            uint xp = (uint)(((uint)Math.Pow(level, 4) + 4 * (uint)Math.Pow(level, 3) + 53 * (uint)Math.Pow(level, 2) - 58 * level)/10);
            xpBox.Text = xp.ToString();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Binary files (*.bin)|*.bin";
            if (d.ShowDialog() == true)
            {
                OpenFile(File.OpenRead(filename = d.FileName));
                Title = originalTitle + " - " + d.SafeFileName;
                isFTP = false;
            }
        }
    }
    public static class Database
    {
        public static List<Item> weapons;
        public static List<Item> skills;
        #region STRINGS
        public static string[] Arcana = new string[]{
            "",
            "Fool",
            "Magician",
            "Priestess",
            "Empress",
            "Emperor",
            "Hierophant",
            "Lovers",
            "Chariot",
            "Justice",
            "Hermit",
            "Fortune",
            "Strength",
            "Hanged Man",
            "Death",
            "Temperance",
            "Devil",
            "Tower",
            "Star",
            "Moon",
            "Sun",
            "Judgement",
            "Aeon",
            "???",
            "World",
            "Jester",
            "Hunger",
            "Aeon",
            "Outsider",
            "1D",
            "1E",
            "1F"
        };
        public static string[] phases = new string[]
            {
            "Early Morning",
            "Morning",
            "Lunchtime",
            "Afternoon",
            "After School",
            "Evening"
            };
            public static string[] party { get;set; } = new string[]
            {
            "Yu Narukami",
            "Yosuke Hanamura",
            "Chie Satonaka",
            "Yukiko Amagi",
            "Rise Kujikawa",
            "Kanji Tatsumi",
            "Naoto Shirogane",
            "Teddie",
            };
            public static string[] personae = new string[]
            {
            "000",
            "Izanagi",
            "Tzitzimitl",
            "Flauros",
            "Loki",
            "Ishtar",
            "Pyro Jack",
            "Jack Frost",
            "Scathach",
            "Rangda",
            "Hachiman",
            "Cu Chulainn",
            "Ose",
            "Kusi Mitama",
            "Apsaras",
            "Ardha",
            "Parvati",
            "Kikuri-Hime",
            "Zaou-Gongen",
            "Sarasvati",
            "Yatsufusa",
            "Cybele",
            "Sraosha",
            "Neko Shogun",
            "Kali",
            "Obariyon",
            "Ukobach",
            "Lamia",
            "Odin",
            "King Frost",
            "Oukuninushi",
            "Undine",
            "Sylph",
            "Forneus",
            "Alraune",
            "Mithra",
            "Daisoujou",
            "Ananta",
            "Futsunushi",
            "Triglav",
            "Raphael",
            "Titania",
            "Oberon",
            "02B",
            "Sandman",
            "Leanan Sidhe",
            "Pixie",
            "Uriel",
            "Surt",
            "Throne",
            "Ares",
            "Titan",
            "034",
            "Ara Mitama",
            "Valkyrie",
            "Melchizedek",
            "Dominion",
            "Siegfried",
            "Virtue",
            "Power",
            "Archangel",
            "Angel",
            "Hitokotonusi",
            "Ippon-Datara",
            "Nebiros",
            "Decarabia",
            "Belphegor",
            "Yomotsu-Shikome",
            "Vetala",
            "Norn",
            "Atropos",
            "Pazuzu",
            "Lachesis",
            "Saki Mitama",
            "Eligor",
            "Clotho",
            "Fortuna",
            "Thor",
            "Mokoi",
            "Abaddon",
            "Belial",
            "Hanuman",
            "Yoshitsune",
            "Mahakala",
            "Attis",
            "Vasuki",
            "Orthrus",
            "Tam Lin",
            "Jinn",
            "Mada",
            "White Rider",
            "Alice",
            "Seth",
            "Mot",
            "Samael",
            "Gdon",
            "Gorgon",
            "Dis",
            "Michael",
            "Byakko",
            "Suzaku",
            "Seiryuu",
            "Nigi Mitama",
            "Genbu",
            "Beelzebub",
            "Mother Harlot",
            "Shiki-Ouji",
            "Lilith",
            "Incubus",
            "Succubus",
            "Lilim",
            "Phoenix",
            "Shiva",
            "Masakado",
            "Ikusa",
            "Yamatano-Orochi",
            "Anzu",
            "Helel",
            "Sandalphon",
            "Black Frost",
            "Garuda",
            "Sui-ki",
            "Ganesha",
            "Isis",
            "Cerberus",
            "Fuu-ki",
            "Setanta",
            "Girimehkala",
            "Nozuchi",
            "Legion",
            "Berith",
            "Saturnus",
            "Vishnu",
            "Barong",
            "Andra",
            "Horus",
            "Narasimha",
            "Senri",
            "Kin-ki",
            "Asura",
            "Metatron",
            "Satan",
            "Gabriel",
            "Hokuto Seikun",
            "Trumpeter",
            "Anubis",
            "Nata Taishi",
            "Ongyo-ki",
            "High Pixie",
            "Yaksini",
            "Xiezhai",
            "Thoth",
            "Cu Sith",
            "Mothman",
            "Oni",
            "Makami",
            "Rakshasa",
            "Matador",
            "Hell Biker",
            "Taowu",
            "Taotie",
            "Pabilsag",
            "Mara",
            "Kartikeya",
            "Baal Zebul",
            "Suparna",
            "Lucifer",
            "Orobas",
            "Atavaka",
            "Hariti",
            "Skadi",
            "Unicorn",
            "Omoikane",
            "Shiisaa",
            "Principality",
            "Kurama Tengu",
            "Yurlungur",
            "Kaiwan",
            "Jatayu",
            "Slime",
            "0B4",
            "0B5",
            "Arahabaki",
            "Hua Po",
            "Alilat",
            "Kohryu",
            "Ghoul",
            "Queen Mab",
            "Ganga",
            "Izanagi-no-Okami",
            "Niddhoggr",
            "Yatagarasu",
            "Jiraiya",
            "Susano-O",
            "Tomoe",
            "Suzuka Gongen",
            "Konohana Sakuya",
            "Amaterasu",
            "Take-Mikazuchi",
            "Rokuten Maoh",
            "Himiko",
            "Kanzeon",
            "Kintoki-Douji",
            "Kamui",
            "Sukuna-Hikona",
            "Yamato Takeru",
            "Magatsu-Izanagi",
            "Takehaya Susano-o",
            "Haraedo-no-Okami",
            "Sumeo-Okami",
            "Takeji Zaiten",
            "Kouzeon",
            "Kamui-Moshiri",
            "Yamato Sumeragi",
            "0D6",
            "0D7",
            "0D8",
            "0D9",
            "0DA",
            "0DB",
            "0DC",
            "0DD",
            "0DE",
            "0DF",
            "Black Izanami",
            "Okami",
            "ÇñÇóÇºÇôÇò",
            "ÇñÇóÇºÇöÇò",
            "ÇñÇóÇºÇòÇò",
            "ÇñÇóÇºÇûÇò",
            "ÇñÇóÇºÇùÇò",
            "ÇñÇóÇºÇÿÇò",
            "Kaguya",
            "Pale Rider",
            "Take-Minakata",
            "Narcissus",
            "Kumbhanda",
            "Gurr",
            "Baphomet",
            "Loa",
            "Chernobog",
            "Quetzalcoatl",
            "Ame-no-Uzume",
            "Seiten Taisei",
            "Kingu",
            "Kushinada",
            "Sati",
            "Raja Naga",
            "Laksmi",
            "Magatsu-Izanagi"
            };
            public static string[] allSkills = new string[]
            {
            "Blank",
            "Agi",
            "Agilao",
            "Agidyne",
            "Maragi",
            "Maragion",
            "Maragidyne",
            "Prominence",
            "Ragnarok",
            "Immortal Flame",
            "Tiny Soul Tomato",
            "Tetracorn",
            "Run Amok",
            "Shake Off",
            "Garu",
            "Garula",
            "Garudyne",
            "Magaru",
            "Magarula",
            "Magarudyne",
            "Divine Vacuum",
            "Panta Rhei",
            "Hiranya Cabbage",
            "Cry of Denial",
            "Hot Lightning",
            "Draining Fog",
            "Bufu",
            "Bufula",
            "Bufudyne",
            "Mabufu",
            "Mabufula",
            "Mabufudyne",
            "Cocytus Pain",
            "Niflheim",
            "Bloody Rain",
            "Red Paprika",
            "Enclosure Shell",
            "Hot Lightning",
            "Shell of Denial",
            "Zio",
            "Zionga",
            "Ziodyne",
            "Mazio",
            "Mazionga",
            "Maziodyne",
            "Jihad",
            "Thunder Reign",
            "Bead Melon",
            "Mid Soul Tomato",
            "Big Soul Tomato",
            "Makaracorn",
            "Megido",
            "Megidola",
            "Megidolaon",
            "Last Resort",
            "Black Viper",
            "Morning Star",
            "White Paprika",
            "Blue Paprika",
            "Green Paprika",
            "Blank",
            "Hama",
            "Hamaon",
            "Mahama",
            "Mahamaon",
            "Eternal White",
            "God's Judgment",
            "Samsara",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Mudo",
            "Mudoon",
            "Mamudo",
            "Mamudoon",
            "Eternal Black",
            "Demonic Judgment",
            "Die for Me!",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Pulinpa",
            "Tentarafoo",
            "Evil Touch",
            "Evil Smile",
            "Ghastly Wail",
            "Balzac",
            "Valiant Dance",
            "Poisma",
            "Poison Mist",
            "Soul Break",
            "Anima Freeze",
            "Enervation",
            "Old One",
            "Galgalim Eyes",
            "Makajam",
            "Foolish Whisper",
            "Foul Breath",
            "Stagnant Air",
            "Life Drain",
            "Spirit Drain",
            "Life Leech",
            "Spirit Leech",
            "Judgement",
            "Balzac",
            "Last Resort",
            "Last Resort",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Kamui Miracle",
            "Blank",
            "Elemental Break",
            "Bash",
            "Cleave",
            "Assault Dive",
            "Sonic Punch",
            "Double Fangs",
            "Kill Rush",
            "Swift Strike",
            "Twin Shot",
            "Fatal End",
            "Mighty Swing",
            "Torrent Shot",
            "Heat Wave",
            "Gigantic Fist",
            "Blade of Fury",
            "Deathbound",
            "Arrow Rain",
            "Akasha Arts",
            "Tempest Slash",
            "Heaven's Blade",
            "Myriad Arrows",
            "God's Hand",
            "Pralaya",
            "Primal Force",
            "Vorpal Blade",
            "Power Slash",
            "Gale Slash",
            "Brave Blade",
            "Herculean Strike",
            "Vicious Strike",
            "Single Shot",
            "Skewer",
            "Poison Skewer",
            "Poison Arrow",
            "Blight",
            "Virus Wave",
            "Skull Cracker",
            "Mind Slice",
            "Hysterical Slap",
            "Crazy Chain",
            "Muzzle Shot",
            "Seal Bomb",
            "Arm Chopper",
            "Atom Smasher",
            "Cell Breaker",
            "Mustard Bomb",
            "Brain Shake",
            "Navas Nebula",
            "Golden Right",
            "Black Spot",
            "Rainy Death",
            "Hassou Tobi",
            "Rampage",
            "Aeon Rain",
            "Agneyastra",
            "Cruel Attack",
            "Vile Assault",
            "Rampage",
            "Attack Up Support",
            "Guard Up Support",
            "Agility Up Support",
            "Charge Support",
            "HP Recovery Support",
            "SP Recovery Support",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Dia",
            "Diarama",
            "Diarahan",
            "Media",
            "Mediarama",
            "Mediarahan",
            "Salvation",
            "Patra",
            "Me Patra",
            "Re Patra",
            "Posumudi",
            "Mutudi",
            "Enradi",
            "Tirundi",
            "Nervundi",
            "Energy Shower",
            "Amrita",
            "Recarm",
            "Samarecarm",
            "Tarunda",
            "Matarunda",
            "Sukunda",
            "Masukunda",
            "Rakunda",
            "Marakunda",
            "Dekunda",
            "Tarukaja",
            "Matarukaja",
            "Sukukaja",
            "Masukukaja",
            "Rakukaja",
            "Marakukaja",
            "Dekaja",
            "Heat Riser",
            "Debilitate",
            "Power Charge",
            "Mind Charge",
            "Tetrakarn",
            "Makarakarn",
            "Tetra Break",
            "Makara Break",
            "Tetraja",
            "Rebellion",
            "Revolution",
            "Fire Break",
            "Ice Break",
            "Wind Break",
            "Elec Break",
            "Red Wall",
            "White Wall",
            "Blue Wall",
            "Green Wall",
            "Trafuri",
            "Recarmdra",
            "Traesto",
            "Youthful Wind",
            "Dragon Hustle",
            "Burning Petals",
            "The Man's Way",
            "Kamui Miracle",
            "Shield of Justice",
            "Complete Analysis",
            "Blank",
            "ÇƒÇÇÇƒÇÇÇƒÇÇÇƒÇÇÇƒ",
            "100",
            "101",
            "102",
            "Waste Money",
            "Yosuke Strike",
            "Galactic Punt",
            "Fan Assault",
            "Atomic Press",
            "Wild Ways",
            "Ultra Trigger",
            "Galaxy Kick",
            "Full Analysis",
            "Third Eye",
            "Weakness Scan",
            "Healing Wave",
            "Relaxing Wave",
            "Analyze",
            "Summon",
            "Enemy Radar",
            "Treasure Radar",
            "Certain Escape",
            "Myriad Truths",
            "Cavalry Attack",
            "Cavalry Attack",
            "Cavalry Attack",
            "Cavalry Attack",
            "Cavalry Attack",
            "Cavalry Attack",
            "All-Out Boost",
            "Rise's Revival",
            "Stamina Song",
            "Vigor Song",
            "Sarx Drop",
            "Medicine",
            "Medical Powder",
            "Spirit Water",
            "Life Stone",
            "Bead",
            "Value Medicine",
            "Medical Kit",
            "Umugi Water",
            "Bead Chain",
            "Pneuma Drop",
            "Snuff Soul",
            "Chewing Soul",
            "Precious Egg",
            "12E",
            "12F",
            "130",
            "131",
            "132",
            "133",
            "134",
            "135",
            "136",
            "137",
            "138",
            "139",
            "13A",
            "13B",
            "13C",
            "13D",
            "13E",
            "13F",
            "140",
            "141",
            "142",
            "143",
            "144",
            "145",
            "146",
            "147",
            "148",
            "149",
            "14A",
            "14B",
            "14C",
            "14D",
            "14E",
            "14F",
            "150",
            "151",
            "152",
            "153",
            "154",
            "155",
            "156",
            "157",
            "158",
            "159",
            "15A",
            "15B",
            "15C",
            "Junes Bomber",
            "Twin Dragons",
            "Beauty & The Beast",
            "Wind of Oblivion",
            "Bottomless Envy",
            "Burn to Ashes",
            "Shivering Rondo",
            "Summon",
            "Summon",
            "Forbidden Murmur",
            "Roar of Wrath",
            "Fanatical Spark",
            "Supreme Insight",
            "Nihil Hand",
            "Ultra Charge",
            "Nullity Guidance",
            "Attack",
            "Whisper",
            "Chant",
            "Prayer",
            "Character Setup",
            "Command",
            "Command",
            "Command",
            "Command",
            "Command",
            "Command",
            "Element Zero",
            "Mute Ray",
            "Quad Converge",
            "Control",
            "Unerring Justice",
            "Nebula Oculus",
            "Quake",
            "Bewildering Fog",
            "Bewildering Fog",
            "Thousand Curses",
            "World's End",
            "Terror Voice",
            "Control",
            "Fury of Yasogami",
            "Bewildering Fog",
            "Summons to Yomi",
            "Kuro Ikazuchi",
            "Oho Ikazuchi",
            "Megidolaon",
            "Thousand Curses",
            "18C",
            "18D",
            "18E",
            "18F",
            "190",
            "191",
            "192",
            "193",
            "194",
            "195",
            "196",
            "197",
            "198",
            "199",
            "19A",
            "19B",
            "19C",
            "19D",
            "19E",
            "19F",
            "1A0",
            "1A1",
            "1A2",
            "1A3",
            "1A4",
            "1A5",
            "1A6",
            "1A7",
            "1A8",
            "1A9",
            "1AA",
            "1AB",
            "1AC",
            "1AD",
            "1AE",
            "1AF",
            "1B0",
            "1B1",
            "1B2",
            "1B3",
            "1B4",
            "1B5",
            "1B6",
            "1B7",
            "Snow Flower Honey",
            "Revival Rope",
            "Sentou Seed",
            "Sentou Petal",
            "Sentou Fruit",
            "Fire Bell",
            "Ice Bell",
            "Wind Bell",
            "Lightning Bell",
            "Flame Dotaku",
            "Frigid Dotaku",
            "Gale Dotaku",
            "Bolt Dotaku",
            "1C5",
            "1C6",
            "1C7",
            "1C8",
            "1C9",
            "1CA",
            "1CB",
            "1CC",
            "1CD",
            "1CE",
            "1CF",
            "1D0",
            "1D1",
            "1D2",
            "1D3",
            "1D4",
            "1D5",
            "1D6",
            "1D7",
            "Resist Physical",
            "Null Physical",
            "Repel Physical",
            "Absorb Physical",
            "Resist Fire",
            "Null Fire",
            "Repel Fire",
            "Absorb Fire",
            "Resist Ice",
            "Null Ice",
            "Repel Ice",
            "Absorb Ice",
            "Resist Elec",
            "Null Elec",
            "Repel Elec",
            "Absorb Elec",
            "Resist Wind",
            "Null Wind",
            "Repel Wind",
            "Absorb Wind",
            "Resist Light",
            "Null Light",
            "Repel Light",
            "Resist Dark",
            "Null Dark",
            "Repel Dark",
            "Null Panic",
            "Null Exhaust",
            "Null Mute",
            "Null Fear",
            "Null Rage",
            "Null Poison",
            "Null Dizzy",
            "Null Enervate",
            "Unshaken Will",
            "Masakados",
            "Dodge Physical",
            "Evade Physical",
            "Dodge Fire",
            "Evade Fire",
            "Dodge Ice",
            "Evade Ice",
            "Dodge Wind",
            "Evade Wind",
            "Dodge Elec",
            "Evade Elec",
            "Angelic Grace",
            "Fire Boost",
            "Fire Amp",
            "Ice Boost",
            "Ice Amp",
            "Elec Boost",
            "Elec Amp",
            "Wind Boost",
            "Wind Amp",
            "Certain Escape",
            "HP Up 1",
            "HP Up 2",
            "HP Up 3",
            "SP Up 1",
            "SP Up 2",
            "SP Up 3",
            "Counter",
            "Counterstrike",
            "High Counter",
            "Regenerate 1",
            "Regenerate 2",
            "Regenerate 3",
            "Invigorate 1",
            "Invigorate 2",
            "Invigorate 3",
            "Growth 1",
            "Growth 2",
            "Growth 3",
            "Auto-Tarukaja",
            "Auto-Rakukaja",
            "Auto-Sukukaja",
            "Alertness",
            "Sharp Student",
            "Apt Pupil",
            "Ali Dance",
            "Firm Stance",
            "Spell Master",
            "Arms Master",
            "HP Favor",
            "SP Favor",
            "Divine Grace",
            "Endure",
            "Enduring Soul",
            "Survive Light",
            "Survive Dark",
            "Auto-Maraku",
            "Auto-Mataru",
            "Auto-Masuku",
            "Panic Boost",
            "Poison Boost",
            "Exhaust Boost",
            "Silence Boost",
            "Fear Boost",
            "Rage Boost",
            "Dizzy Boost",
            "Enervate Boost",
            "Ailment Boost",
            "Hama Boost",
            "Mudo Boost",
            "Endure Light",
            "Endure Dark",
            "Cool Breeze",
            "Victory Cry",
            "Resist Poison",
            "Resist Panic",
            "Resist Fear",
            "Resist Exhaust",
            "Resist Enervate",
            "Resist Rage",
            "Resist Dizzy",
            "Resist Mute",
            "HP Amp",
            "SP Amp",
            "Fast Heal",
            "Insta-Heal",
            "Infinite Endure",
            "Izanami Endure",
            "Alienation Wall",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Mudo",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            "Blank",
            };
            public static string[] allItems = new string[]
            {
            "Blank",
            "Golf Club",
            "Titanium Club",
            "5-Iron",
            "Imitation Katana",
            "Musashi Shinai",
            "Long Sword",
            "Zweihander",
            "Iai Katana",
            "Bastard Sword",
            "Gothic Sword",
            "Type-98 Gunto",
            "Downpour Sword",
            "Edge",
            "Kage-Dachi",
            "Great Sword",
            "Midare Hamon",
            "Anglaise",
            "Kakitsubata",
            "Gardenia Sword",
            "Kenka",
            "Krieg",
            "Kijintou",
            "Number One",
            "Gaia Sword",
            "Tsubaki-Otoshi",
            "Myth-like Sword",
            "Shichisei-Ken",
            "El Caliente",
            "Yahiro Sword",
            "Triumph",
            "Tajikarao Sword",
            "Futsuno Mitama",
            "Soul Crusher",
            "Wooden Bat",
            "Metal Bat",
            "Blade of Totsuka",
            "0x025",
            "0x026",
            "Monkey Wrench",
            "Titanium Wrench",
            "Skill Spanner",
            "Hunting Nata",
            "Kunai",
            "Poison Kunai",
            "Reign Skinner",
            "Santou",
            "Kozuka",
            "Throwing Kunai",
            "Kris Naga",
            "Kaiken",
            "Bashin",
            "Yashioori Dagger",
            "Yashima",
            "En-Giri",
            "Bloody Kunai",
            "Fearful Kunai",
            "Castilla Knife",
            "Ohorinomikoto",
            "Flying Kato",
            "Basho",
            "Thief Dagger",
            "Fuuma Kotaro",
            "Militia Dagger",
            "Blitz Knife",
            "Hattori",
            "Ogre Tooth",
            "Rappa",
            "Kashin Koji",
            "Kitchen Knife",
            "Malakh",
            "0x047",
            "0x048",
            "0x049",
            "0x04A",
            "0x04B",
            "0x04C",
            "Kyo Sensu",
            "Silk Fan",
            "Suzaku Feather",
            "Arazuyu Fan",
            "Tessen",
            "Masquerade",
            "Akisame Fan",
            "Hanachirusato",
            "Madam's Charm",
            "Inversion Fan",
            "Amagyou Fan",
            "Mogari-Bue",
            "Fickle Madam",
            "Ganar",
            "Harlot's Mercy",
            "Courtesia",
            "Uhi Fan",
            "Adoracion",
            "Suzumushi",
            "Duchess",
            "Noblesse Oblige",
            "Hototogisu",
            "Secret Fan",
            "Kacho Fugetsu",
            "Pieta",
            "Yume no Ukihashi",
            "Noh Fan",
            "Boundless Sea",
            "0x069",
            "0x06A",
            "0x06B",
            "0x06C",
            "0x06D",
            "0x06E",
            "0x06F",
            "Leather Shoes",
            "Red-Leaf Gusoku",
            "Shield Boots",
            "Nanman Gusoku",
            "Punk Shoes",
            "Hard Boots",
            "Skill Greaves",
            "Adios Shoes",
            "Nice Shoes",
            "Cowboy Boots",
            "Heavy Heels",
            "Bishamonten",
            "Amami Legs",
            "Furinkazan",
            "Bucking Broncos",
            "Sleipnir",
            "Four Beasts",
            "Vampire Shoes",
            "Mjolnir Boots",
            "Vidar's Boots",
            "Kintabi Gusoku",
            "Stella Greaves",
            "Hero Legs",
            "Demon Boots",
            "Kehaya",
            "Gigant Fall",
            "Peerless Heels",
            "Judgment Boots",
            "Steel Slippers",
            "Platform Sneaks",
            "Moses Sandals",
            "0x08F",
            "0x090",
            "0x091",
            "0x092",
            "0x093",
            "0x094",
            "0x095",
            "Folding Chair",
            "Steel Plate",
            "Hard Board",
            "Photon Plate",
            "Power Plate",
            "Alloyed Plate",
            "Thunder Plate",
            "Gorgon Plate",
            "Scutum",
            "Barbarian Shield",
            "Golden Plate",
            "Demon Shield",
            "Mega Buckler",
            "Oni-Gawara",
            "Bath Lid",
            "Death Scudetto",
            "Phalanx",
            "Sol Breaker",
            "Asturias",
            "Black Targe",
            "Aegis Shield",
            "Dullahan",
            "Christ Mirror",
            "Yasogami Desk",
            "Iron Plate",
            "Perun Plate",
            "0x0B0",
            "0x0B1",
            "0x0B2",
            "0x0B3",
            "0x0B4",
            "0x0B5",
            "0x0B6",
            "Nambu 2",
            "Peacemaker",
            "Raging Bull",
            "Crimson Dirge",
            "",
            "Magatsu Kiba",
            "Chrome Heart",
            "Jovian Thunder",
            "Unlimited",
            "Camel Red",
            "Algernon",
            "From Zero",
            "Judge of Hell",
            "Athena Kiss R",
            "Black Hole",
            "0x0C6",
            "0x0C7",
            "0x0C8",
            "0x0C9",
            "0x0CA",
            "0x0CB",
            "0x0CC",
            "0x0CD",
            "0x0CE",
            "0x0CF",
            "0x0D0",
            "0x0D1",
            "0x0D2",
            "0x0D3",
            "0x0D4",
            "0x0D5",
            "0x0D6",
            "0x0D7",
            "0x0D8",
            "Spikey Punch",
            "Mewling Claw",
            "Bear Claw",
            "Drunken Fist",
            "Typhoon Claw",
            "Mail Duster",
            "Air Break",
            "Poison Claw",
            "Assault Spike",
            "Gehenna Claw",
            "Cute Assassin",
            "Chain Glove",
            "Fuuma Bundou",
            "Strega Claw",
            "Pure Assassin",
            "Needle Spike",
            "Seiryu Claw",
            "Jakotsu Claw",
            "Shitisei Jakotsu",
            "Platinum Claw",
            "The Ripper",
            "Seireiga",
            "0x0EF",
            "0x0F0",
            "0x0F1",
            "0x0F2",
            "0x0F3",
            "0x0F4",
            "0x0F5",
            "0x0F6",
            "0x0F7",
            "0x0F8",
            "0x0F9",
            "0x0FA",
            "0x0FB",
            "0x0FC",
            "0x0FD",
            "Bare Hands",
            "0x0FF",
            "Blank",
            "T-Shirt",
            "Long T-Shirt",
            "Tank Top",
            "Lace Blouse",
            "Gothic Shirt",
            "Skull T-Shirt",
            "Pretty Suit",
            "Lace Camisole",
            "0x109",
            "Chain Mail",
            "Kevlar Vest",
            "Survival Guard",
            "Metal Jacket",
            "Lion Happi",
            "Charm Robe",
            "0x110",
            "0x111",
            "0x112",
            "0x113",
            "0x114",
            "0x115",
            "0x116",
            "0x117",
            "0x118",
            "0x119",
            "0x11A",
            "0x11B",
            "0x11C",
            "0x11D",
            "0x11E",
            "Knifeproof Coat",
            "Battle Camisole",
            "Doumaru",
            "Miori Shirt",
            "0x123",
            "0x124",
            "Desperate Plate",
            "Combat Dress",
            "Gentleman's Tux",
            "Hard Bolero",
            "0x129",
            "0x12A",
            "0x12B",
            "0x12C",
            "0x12D",
            "0x12E",
            "0x12F",
            "0x130",
            "0x131",
            "0x132",
            "Black Stone Mail",
            "Wolf Tunic",
            "Knight Scale",
            "Action Vest",
            "0x137",
            "0x138",
            "0x139",
            "0x13A",
            "Hard Armor",
            "Ame-Otoko",
            "Zero Kosode",
            "0x13E",
            "0x13F",
            "0x140",
            "0x141",
            "0x142",
            "0x143",
            "0x144",
            "0x145",
            "0x146",
            "0x147",
            "Plate Mail",
            "Capital Robe",
            "Ama-Gakure",
            "Amakaze Happi",
            "0x14C",
            "0x14D",
            "Passion Sweats",
            "Steel Panier",
            "Jingi Fundoshi",
            "Breeze Tutu",
            "Haikara Shirt",
            "0x153",
            "0x154",
            "0x155",
            "0x156",
            "0x157",
            "0x158",
            "0x159",
            "0x15A",
            "Kuroito-odoshi",
            "Divine Blouse",
            "0x15D",
            "Purple Suit",
            "Talisman Cape",
            "Amamusha Armor",
            "Invincible Mini",
            "Niagra Climber",
            "Armada Bustier",
            "Hurricane Coat",
            "Uin Haori",
            "Paladin Armor",
            "0x167",
            "0x168",
            "0x169",
            "0x16A",
            "0x16B",
            "0x16C",
            "0x16D",
            "0x16E",
            "Dragon Scale",
            "Mythos Robe",
            "Stylish Kimono",
            "Lan Ling Wang",
            "0x173",
            "Ame-agari Kesshi",
            "Elint Duffle",
            "Oracle Gown",
            "Rune Dress",
            "Charm Drape",
            "Peach Battlesuit",
            "Red Battlesuit",
            "0x17B",
            "0x17C",
            "0x17D",
            "0x17E",
            "0x17F",
            "0x180",
            "0x181",
            "0x182",
            "1000-Stud Coat",
            "Kikusui Awase",
            "Mikagura Vest",
            "Kotodama Cape",
            "Komaryo Uchinugi",
            "Shinra Robe",
            "0x189",
            "Surcoat",
            "Mizuha Armor",
            "Tsukuyomi Noshi",
            "Amaterasu Hitoe",
            "Angel Skirt",
            "0x18F",
            "0x190",
            "0x191",
            "0x192",
            "0x193",
            "0x194",
            "0x195",
            "0x196",
            "Opera Coat",
            "Yomi Sleeves",
            "Lorica Hamata",
            "Emery Meisen",
            "Full Jin-Baori",
            "Haten Robe",
            "0x19D",
            "Nubatama Suit",
            "Uzume Robe",
            "Sonidori Wear",
            "Mandala Robe",
            "Godly Robe",
            "0x1A3",
            "White Joue",
            "Fire Joue",
            "Ice Joue",
            "Wind Joue",
            "Lightning Joue",
            "Fire Repel Joue",
            "Ice Repel Joue",
            "Wind Repel Joue",
            "Elec Repel Joue",
            "Fire Drain Joue",
            "Ice Drain Joue",
            "Wind Drain Joue",
            "Elec Drain Joue",
            "0x1B1",
            "0x1B2",
            "0x1B3",
            "0x1B4",
            "0x1B5",
            "0x1B6",
            "0x1B7",
            "0x1B8",
            "0x1B9",
            "0x1BA",
            "0x1BB",
            "0x1BC",
            "0x1BD",
            "0x1BE",
            "0x1BF",
            "0x1C0",
            "0x1C1",
            "0x1C2",
            "0x1C3",
            "0x1C4",
            "0x1C5",
            "0x1C6",
            "0x1C7",
            "0x1C8",
            "0x1C9",
            "0x1CA",
            "0x1CB",
            "0x1CC",
            "0x1CD",
            "0x1CE",
            "0x1CF",
            "0x1D0",
            "0x1D1",
            "0x1D2",
            "0x1D3",
            "0x1D4",
            "0x1D5",
            "0x1D6",
            "0x1D7",
            "0x1D8",
            "0x1D9",
            "0x1DA",
            "0x1DB",
            "0x1DC",
            "0x1DD",
            "0x1DE",
            "0x1DF",
            "0x1E0",
            "0x1E1",
            "0x1E2",
            "0x1E3",
            "0x1E4",
            "0x1E5",
            "0x1E6",
            "0x1E7",
            "0x1E8",
            "0x1E9",
            "0x1EA",
            "0x1EB",
            "0x1EC",
            "0x1ED",
            "0x1EE",
            "0x1EF",
            "0x1F0",
            "0x1F1",
            "0x1F2",
            "0x1F3",
            "0x1F4",
            "0x1F5",
            "0x1F6",
            "0x1F7",
            "0x1F8",
            "0x1F9",
            "0x1FA",
            "0x1FB",
            "0x1FC",
            "0x1FD",
            "0x1FE",
            "0x1FF",
            "Blank",
            "Wristwatch",
            "Plain Ring",
            "Aluminum Badge",
            "Hair Band",
            "Spiral Earrings",
            "Silver Locket",
            "Bear Ears",
            "Ribbon",
            "0x209",
            "Worn Rosary",
            "Fire Vow",
            "Flame Vow",
            "Blaze Vow",
            "Oven Vow",
            "Kagutsuchi Vow",
            "Ice Vow",
            "Snow Vow",
            "Blizzard Vow",
            "Icicle Vow",
            "Kuraokami Vow",
            "Wind Vow",
            "Gust Vow",
            "Storm Vow",
            "Gale Vow",
            "Shinatobe Vow",
            "Thunder Vow",
            "Spark Vow",
            "Volt Vow",
            "Lightning Vow",
            "Takefutsu Vow",
            "Worn Talisman",
            "Fire Pin",
            "Flame Pin",
            "Blaze Pin",
            "Oven Pin",
            "Kagutsuchi Pin",
            "Ice Pin",
            "Snow Pin",
            "Blizzard Pin",
            "Icicle Pin",
            "Kuraokami Pin",
            "Wind Pin",
            "Gust Pin",
            "Storm Pin",
            "Gale Pin",
            "Shinatobe Pin",
            "Thunder Pin",
            "Spark Pin",
            "Volt Pin",
            "Lightning Pin",
            "Takefutsu Pin",
            "Worn Evil Eye",
            "Marksman Eye 1",
            "Marksman Eye 2",
            "Marksman Eye 3",
            "Marksman Eye 4",
            "Marksman Eye 5",
            "Old Prayer Beads",
            "Sight Beads 1",
            "Sight Beads 2",
            "Sight Beads 3",
            "Sight Beads 4",
            "Sight Beads 5",
            "Disinfectant",
            "Tranquil Buddha",
            "Bravery Vessel",
            "Healthy Recipe",
            "Kid's Hachimaki",
            "Cool Beads",
            "Safety Belt",
            "Disguise Mask",
            "Gas Mask",
            "Confusion Hat",
            "Bold Ball",
            "Healthy Homa",
            "Pitch Pipes",
            "Frozen Rose",
            "Alarm Clock",
            "Mask of Truth",
            "Book of Ruin",
            "Book of Serenity",
            "Book of the Void",
            "White Feather",
            "Black Feather",
            "Falcon Eye",
            "Eagle Eye",
            "Fire Suppressor",
            "Eye of Flame",
            "Ice Suppressor",
            "Eye of Ice",
            "Wind Suppressor",
            "Eye of Wind",
            "Volt Suppressor",
            "Eye of Thunder",
            "Omnipotent Orb",
            "Cat Whisker",
            "0x261",
            "0x262",
            "0x263",
            "0x264",
            "0x265",
            "0x266",
            "Revenge Ring",
            "Reprisal Chain",
            "Retribution Mask",
            "Land Badge",
            "Earth Badge",
            "Space Badge",
            "Mage's Mark",
            "Sorcerer's Mark",
            "Wizard's Mark",
            "Awareness Note",
            "Sigma Drive",
            "Omega Drive",
            "Barbaric Bracers",
            "Divine Pillar",
            "Chakra Ring",
            "Rudra Ring",
            "Blessed Hands",
            "Patient Collar",
            "Rebirth Prophecy",
            "Headband",
            "Power Belt",
            "Power Tasuki",
            "Spirit Hachimaki",
            "Festival Drum",
            "Power Muscle",
            "Lizard Charm",
            "Crow Charm",
            "Bat Charm",
            "Black Cat Charm",
            "Witch Charm",
            "Demon Charm",
            "Amulet",
            "Talisman",
            "Guard Amulet",
            "Iron Charm",
            "Guard Rosary",
            "Defense Essence",
            "Feather Strap",
            "Wing Strap",
            "Flight Strap",
            "Steed Strap",
            "Pegasus Strap",
            "Speed Star",
            "Lucky Coin",
            "Rabbit Foot",
            "Silver Spoon",
            "Maneki Neko",
            "Wooden Ebisu",
            "Lucky Seven",
            "Feng Shui Brace",
            "Kimyaku Brace",
            "Ryumyaku Brace",
            "Reisen Brace",
            "Ouryu Brace",
            "God's Love",
            "Vigor Fob",
            "Life Fob",
            "Amaterasu Fob",
            "Plum Potpourri",
            "Soul Potpourri",
            "Moon Potpourri",
            "Fire Bangle",
            "Agni Bangle",
            "Ice Bangle",
            "Varuna Bangle",
            "Thunder Bangle",
            "Indra Bangle",
            "Wind Bangle",
            "Vayu Bangle",
            "0x2AC",
            "Parting Stone",
            "0x2AE",
            "Pain Stone",
            "Grief Stone",
            "Sorrow Stone",
            "Love Stone",
            "Suffering Stone",
            "Anger Stone",
            "Loneliness Stone",
            "Invitation Stone",
            "0x2B7",
            "0x2B8",
            "0x2B9",
            "0x2BA",
            "0x2BB",
            "0x2BC",
            "0x2BD",
            "0x2BE",
            "0x2BF",
            "0x2C0",
            "0x2C1",
            "0x2C2",
            "0x2C3",
            "0x2C4",
            "0x2C5",
            "0x2C6",
            "0x2C7",
            "0x2C8",
            "0x2C9",
            "0x2CA",
            "0x2CB",
            "0x2CC",
            "0x2CD",
            "0x2CE",
            "0x2CF",
            "0x2D0",
            "0x2D1",
            "0x2D2",
            "0x2D3",
            "0x2D4",
            "0x2D5",
            "0x2D6",
            "0x2D7",
            "0x2D8",
            "0x2D9",
            "0x2DA",
            "0x2DB",
            "0x2DC",
            "0x2DD",
            "0x2DE",
            "0x2DF",
            "0x2E0",
            "0x2E1",
            "0x2E2",
            "0x2E3",
            "0x2E4",
            "0x2E5",
            "0x2E6",
            "0x2E7",
            "0x2E8",
            "0x2E9",
            "0x2EA",
            "0x2EB",
            "0x2EC",
            "0x2ED",
            "0x2EE",
            "0x2EF",
            "0x2F0",
            "0x2F1",
            "Portrait Medal",
            "Paper Armband",
            "Bead Ring",
            "Partial Award",
            "Reader King",
            "Macho Medal",
            "Fluffy Muffler",
            "Leather Keychain",
            "Silver Bangle",
            "Handmade Watch",
            "Rugged Ring",
            "Fancy Mini-Car",
            "Hand-Knit Mitten",
            "0x2FF",
            "Blank",
            "Peach Seed",
            "Medicine",
            "Ointment",
            "Antibiotic Gel",
            "Life Stone",
            "Bead",
            "Value Medicine",
            "Medical Kit",
            "Macca Leaf",
            "Bead Chain",
            "Soul Drop",
            "Snuff Soul",
            "Chewing Soul",
            "Soul Food",
            "Revival Bead",
            "Balm of Life",
            "Royal Jelly",
            "Dokudami Tea",
            "Mouthwash",
            "Sedative",
            "Stimulant",
            "Goho-M",
            "Vanish Ball",
            "Soma",
            "Amrita Soda",
            "Hiranya",
            "Muscle Drink",
            "Odd Morsel",
            "Rancid Gravy",
            "Moon Tsukubame",
            "Magic Mirror",
            "Physical Mirror",
            "Assault Signal",
            "Super Sonic",
            "Diamond Shield",
            "Purifying Water",
            "Purifying Salt",
            "Firecracker",
            "San-zun Tama",
            "Ice Cube",
            "Dry Ice",
            "Pinwheel",
            "Yashichi",
            "Ball Lightning",
            "Tesla Coil",
            "Smart Bomb",
            "Segaki Rice",
            "Curse Paper",
            "Hell Magatama",
            "Cyclone Magatama",
            "Frost Magatama",
            "Arc Magatama",
            "Amethyst",
            "Aquamarine",
            "Emerald",
            "Onyx",
            "Opal",
            "Garnet",
            "Sapphire",
            "Diamond",
            "Turquoise",
            "Topaz",
            "Pearl",
            "Homunculus",
            "Chest Key",
            "Junes Lunch",
            "Steak Skewer DX",
            "Amagi Manju",
            "Mom's Lunch",
            "Mini Mechavator",
            "Sample Food Set",
            "Cough Drop",
            "Meat Gum",
            "Quelorie Magic",
            "Bone Senbei",
            "Milk Chocolate",
            "Yummy Gummy",
            "Ruby",
            "Tater Longs",
            "TaP Soda",
            "Prize Sticker",
            "Super Croquette",
            "Steak Croquette",
            "Steak Skewer",
            "Dr",
            "Second Maid",
            "The Natural",
            "Orange Smash",
            "Yukiko's Lunch",
            "Chie's Muffin",
            "Naoto's Candy",
            "Pulsating Stone",
            "Mystical Scarab",
            "Olympic Tape",
            "Uplifting Radio",
            "Spirit Radio",
            "Diet Food",
            "Slimming Food",
            "Heavy Soup",
            "Giant Candy",
            "Wasabi Jelly",
            "Herbal Pill",
            "Longevity Pill",
            "0x368",
            "Bug Bait",
            "Luxurious Coffee",
            "Power Mountain Blend",
            "Magic Mountain Blend",
            "Tough Mountain Blend",
            "Speed Mountain Blend",
            "Lucky Mountain Blend",
            "0x370",
            "Fire Breaker",
            "Ice Breaker",
            "Wind Breaker",
            "Lightning Breaker",
            "Sacred Branch",
            "0x376",
            "0x377",
            "0x378",
            "0x379",
            "0x37A",
            "0x37B",
            "0x37C",
            "0x37D",
            "0x37E",
            "0x37F",
            "0x380",
            "0x381",
            "0x382",
            "0x383",
            "0x384",
            "0x385",
            "0x386",
            "0x387",
            "0x388",
            "0x389",
            "0x38A",
            "0x38B",
            "Bread Crumbs",
            "Tatsuhime Ladybug",
            "Yaso Locust",
            "Meiou Cricket",
            "Inaba Jewel Beetle",
            "Daimyo Grasshopper",
            "Heike Stag Beetle",
            "Genji Beetle",
            "0x394",
            "0x395",
            "0x396",
            "0x397",
            "0x398",
            "0x399",
            "0x39A",
            "Passion Candy",
            "Sincerity Cookie",
            "Love Marshmallow",
            "Rise's Flan",
            "0x39F",
            "Life Stone",
            "Revival Bead",
            "Sentou Seed",
            "Sentou Petal",
            "Sentou Fruit",
            "Fire Bell",
            "Ice Bell",
            "Wind Bell",
            "Lightning Bell",
            "Flame Dotaku",
            "Frigid Dotaku",
            "Gale Dotaku",
            "Bolt Dotaku",
            "0x3AD",
            "0x3AE",
            "Mini Melon Bread",
            "Choco Melon Bread",
            "Cream Melon Bread",
            "Mini Cornet",
            "Yakisoba Cornet",
            "Red Bean Cornet",
            "Mini Anpan",
            "Mochi Anpan",
            "Cream Anpan",
            "White Karinto",
            "Rice Chocolate",
            "Sour Konbu",
            "Plum Candy",
            "Rainbow Konpeito",
            "Quelorie Mate",
            "Corn Potage Cracker",
            "Blue Pepper Chips",
            "Blue Cheese Chips",
            "Seaweed Cracker",
            "Kampo Chocolate",
            "Checkerboard Cookie",
            "Caramel Drop",
            "Fruit Candy",
            "Rich Milk Candy",
            "0x3C7",
            "0x3C8",
            "0x3C9",
            "0x3CA",
            "0x3CB",
            "0x3CC",
            "0x3CD",
            "0x3CE",
            "0x3CF",
            "0x3D0",
            "0x3D1",
            "Angel Statue",
            "Demon Statue",
            "Ritz Wire",
            "Trio Wig",
            "Fitting Board",
            "Old Key",
            "Suspicious Pole",
            "Coal",
            "Reflecting Board",
            "Crooked Cross",
            "Grand Horn",
            "Flower Brooch",
            "Charmed Veil",
            "Eternal Lamp",
            "Leaf Pochette",
            "Culurium",
            "Crystal Ball",
            "Fine Coal",
            "High-Speed Gear",
            "Branch Headband",
            "Training Shell",
            "Animal Guide",
            "Classy Lumber",
            "Old Ore",
            "Modest Lamp",
            "Mori Ranmaru",
            "Sea Guardian",
            "Meguro Tuna",
            "Drifting Trash",
            "0x3EF",
            "Guardian",
            "Huge Fish",
            "Inaba Trout",
            "Amber Seema",
            "Genji Ayu",
            "Red Goldfish",
            "Hachiro Octopus",
            "0x3F7",
            "Bait",
            "0x3F9",
            "0x3FA",
            "0x3FB",
            "0x3FC",
            "0x3FD",
            "0x3FE",
            "0x3FF",
            "Blank",
            "Velvet Key",
            "Mitsuo's Photo",
            "0x403",
            "0x404",
            "0x405",
            "0x406",
            "0x407",
            "0x408",
            "0x409",
            "0x40A",
            "0x40B",
            "0x40C",
            "0x40D",
            "0x40E",
            "0x40F",
            "0x410",
            "0x411",
            "0x412",
            "0x413",
            "0x414",
            "0x415",
            "0x416",
            "0x417",
            "Spiral Bookmark",
            "Orb of Sight",
            "Aqua Invitation",
            "0x41B",
            "0x41C",
            "Fishhook",
            "Deep-Sea Rod",
            "Butterfly Net",
            "River Rod",
            "Angler's Set",
            "0x422",
            "0x423",
            "0x424",
            "0x425",
            "0x425",
            "0x427",
            "0x428",
            "0x429",
            "0x42A",
            "0x42B",
            "0x42C",
            "0x42D",
            "0x42E",
            "0x42F",
            "0x430",
            "Scooter License",
            "Glass Key",
            "Bathhouse Key",
            "Research Card",
            "Leader Card",
            "Orb of Darkness",
            "0x437",
            "0x438",
            "0x439",
            "0x43A",
            "0x43B",
            "0x43C",
            "0x43D",
            "0x43E",
            "0x43F",
            "0x440",
            "0x441",
            "0x442",
            "0x443",
            "0x444",
            "0x445",
            "0x446",
            "0x447",
            "0x448",
            "0x449",
            "0x44A",
            "0x44B",
            "0x44C",
            "0x44D",
            "0x44E",
            "0x44F",
            "0x450",
            "0x451",
            "0x452",
            "0x453",
            "0x454",
            "0x455",
            "0x456",
            "0x457",
            "0x458",
            "0x459",
            "0x45A",
            "0x45B",
            "0x45C",
            "0x45D",
            "0x45E",
            "0x45F",
            "0x460",
            "0x461",
            "0x462",
            "0x463",
            "Velvet Ticket",
            "Velvet Card",
            "Velvet Pass",
            "Velvet VIP",
            "0x468",
            "0x469",
            "0x46A",
            "0x46B",
            "0x46C",
            "0x46D",
            "0x46E",
            "0x46F",
            "Expert Study Methods",
            "English Made Easy",
            "100 Ghost Stories",
            "Office Work Manual",
            "Easy Origami",
            "0x475",
            "0x476",
            "0x477",
            "0x478",
            "Food Fight!",
            "Beginner Fishing",
            "Expert Fishing",
            "Catching Bugs",
            "World Class Bugs",
            "Home Gardening",
            "Hyperspeed Reading",
            "0x480",
            "0x481",
            "0x482",
            "0x483",
            "0x484",
            "0x485",
            "0x486",
            "0x487",
            "0x488",
            "0x489",
            "0x48A",
            "0x48B",
            "0x48C",
            "0x48D",
            "0x48E",
            "0x48F",
            "0x490",
            "0x491",
            "0x492",
            "0x493",
            "0x494",
            "0x495",
            "0x496",
            "0x497",
            "0x498",
            "0x499",
            "0x49A",
            "0x49B",
            "0x49C",
            "0x49D",
            "Dummy Compendium 1",
            "Dummy Compendium 2",
            "Buddy's Bandage",
            "Shrine Charm",
            "Spiral Brooch",
            "Cute Strap",
            "Coffee Mug",
            "Signed Photo",
            "Wristbands",
            "Family Picture",
            "Gratitude Ema",
            "Detective Badge",
            "Letter to Kou",
            "Spike Brush",
            "Junes Receipt",
            "Old Fountain Pen",
            "Clover Bookmark",
            "Hospital ID",
            "Test Results",
            "Compact",
            "Annotated Script",
            "Handmade Ticket",
            "0x4B4",
            "0x4B5",
            "0x4B6",
            "Adachi's Letter",
            "Adachi's Number",
            "Old Bamboo Comb",
            "0x4BA",
            "0x4BB",
            "0x4BC",
            "0x4BD",
            "0x4BE",
            "0x4BF",
            "0x4C0",
            "0x4C1",
            "0x4C2",
            "0x4C3",
            "0x4C4",
            "0x4C5",
            "0x4C6",
            "0x4C7",
            "Husband's Letters",
            "Hisano's Letters",
            "Family Photo",
            "0x4CB",
            "Handkerchief",
            "White Card",
            "Round Wallet",
            "Square Wallet",
            "Small Package",
            "Tankiriman Sticker",
            "Unfinished Model",
            "MF-06S Brahman",
            "Unfinished Model",
            "Heavy-Armor Agni",
            "Mokoi Doll",
            "Unfinished Model",
            "Melee Harihara",
            "Unfinished Model",
            "Turbo Recon Dyaus",
            "Unfinished Model",
            "Mobile Model Varna",
            "Unfinished Model",
            "D-Type Prithvi",
            "0x4DF",
            "0x4E0",
            "0x4E1",
            "0x4E2",
            "0x4E3",
            "0x4E4",
            "0x4E5",
            "0x4E6",
            "0x4E7",
            "0x4E8",
            "0x4E9",
            "0x4EA",
            "The Lovely Man",
            "Forever Macho",
            "Man of History",
            "Man-God",
            "Farewell to Man",
            "Off Today",
            "Short on Cash",
            "Changing Careers",
            "Sensei's Friends",
            "The Final Lesson",
            "The O-Cha Way",
            "The Gentle Way",
            "The Divine Way",
            "The Ramen Way",
            "The Punk's Way",
            "Witch Detective",
            "Poly-land",
            "Guide to Pests",
            "Picross Rules!",
            "Who Am I?",
            "0x4FF",
            "Blank",
            "Coward's Orb",
            "Selfish Ornament",
            "4th Girl's Talc",
            "Notched Heart",
            "Light Iron",
            "Thick Hide",
            "Glossy Clasp",
            "Electric Rock",
            "Magic Cross",
            "Brave Lumber",
            "Divine Bark",
            "Thought Cross",
            "Supple Metal",
            "Smooth Fabric",
            "Smart Clasp",
            "Living Metal",
            "Big Incisor",
            "Sturdy Molar",
            "Sharp Premolar",
            "Diamond Canine",
            "Brutal Orb",
            "Powered Iron",
            "4th Boy's Talc",
            "Bluff Mask",
            "Curious Line",
            "Rare Poncho",
            "Orb of Love",
            "Klein Bottle",
            "Paleograph",
            "Crimson Cover",
            "Word String",
            "Prophecy Orb",
            "Decorative Stone",
            "Heavy Iron Lump",
            "Trial Obsidian",
            "Calm Marble",
            "Force Rock",
            "Idea Paper",
            "Firm Cloth",
            "Guard Stationery",
            "Oracle Textile",
            "Orb of Vanity",
            "Idle Stone Iron",
            "3rd Girl's Talc",
            "Birdcage Key",
            "Haori Iron",
            "Jet Black Steel",
            "God Tailfeather",
            "Death Sentence",
            "Hard Rock",
            "Stone Hide",
            "Porcelain Statue",
            "Silver Claw",
            "Fur Clasp",
            "Mink Fur",
            "Queen's Nail",
            "Silver Fox Fur",
            "Poison Flower",
            "Quiet Bouquet",
            "Alluring Lily",
            "Royal Blossom",
            "Orb of Pride",
            "Tortoise Shell",
            "3rd Boy's Talc",
            "Fruit of Love",
            "Sharp Horn",
            "Safety Angle",
            "Golden Horn",
            "Platinum Shell",
            "Hard Horn",
            "Bronze Reins",
            "Steel Reins",
            "Black Reins",
            "Platinum Reins",
            "Curious Boulder",
            "Power Rock",
            "Rare Raincoat",
            "Snow Stone",
            "Moustache Fiber",
            "Beard Fiber",
            "Dignified Lump",
            "Silver String",
            "Orb of Sloth",
            "Dependent Iron",
            "2nd Girl's Talc",
            "Malice Fragment",
            "Wonder Cloth",
            "Gray Shackle",
            "Cascade String",
            "Ill Will Claw",
            "Platinum Crown",
            "Dogma Clasp",
            "Sacrilege Iron",
            "Ruinous Crown",
            "Crown of Truth",
            "Alloy Signature",
            "Silver Signature",
            "Black Signature",
            "Gold Signature",
            "Tough Hide",
            "Tanned Hide",
            "Compacted Metal",
            "Platinum Lump",
            "Orb of Betrayal",
            "Anguish Stone",
            "2nd Boy's Talc",
            "Loose Parts",
            "Cloth Wings",
            "Chirping Wings",
            "Crystal of Light",
            "Pure White Bead",
            "Passion Clasp",
            "Blade Metal",
            "Yellow Cord",
            "Hemp Cloth",
            "Love Clasp",
            "Jet Black Scale",
            "Venomous Fang",
            "Scorching Scale",
            "Scaly Lump",
            "Forbidden Scale",
            "Shiny Scale",
            "Flame Scale",
            "Miracle Scale",
            "Love Scale",
            "Breakdown Orb",
            "Insolence Steel",
            "1st Girl's Talc",
            "0x581",
            "Zero Cloth",
            "Bushy Fur",
            "Flame Steel",
            "Brilliant Armor",
            "Iron Gear",
            "Fixing Bolt",
            "Torrent Gear",
            "Platinum Gear",
            "Sheet Metal",
            "Super-Alloy",
            "Black Sheet",
            "Judgement Shot",
            "Flame Sheet",
            "Bull's-Eye Shot",
            "Iron Barrel",
            "Steel Barrel",
            "Golden Barrel",
            "Unknown Barrel",
            "Genuine Orb",
            "Source Lump",
            "1st Boy's Talc",
            "0x597",
            "Zero Board",
            "Rain Steel",
            "Rain Metal",
            "Cascade Metal",
            "Proof of Spirit",
            "Proof of Fight",
            "Proof of Passion",
            "Claw of Myth",
            "Formidable Proof",
            "Life Collar",
            "Steel Collar",
            "Demon Collar",
            "Platinum Fur",
            "Merciless Cord",
            "Azalea Cord",
            "Ayanishiki",
            "Oguruma Brocade",
            "Red-Gold Cord",
            "Orb of Greed",
            "Submission Shard",
            "Aloof Myrrh",
            "0x5AD",
            "Black Lamp",
            "Karma Lamp",
            "Wrought Lamp",
            "Devilish Feather",
            "Golden Cloth",
            "Magic Cloth",
            "Treasure Cloth",
            "Prime Hide",
            "Demon Cloth",
            "Iron Eyeball",
            "Gazing Clasp",
            "Golden Film",
            "Mysterious Eye",
            "Windcutter Cloth",
            "Practical Cloth",
            "Knowledge Staff",
            "Sephirot Staff",
            "Error-Prone Orb",
            "Accident Crystal",
            "Lucky Myrrh",
            "0x5C2",
            "Power String",
            "Pure Iron Lump",
            "Silver Lump",
            "Light String",
            "Mental Thread",
            "Cursed Stone",
            "Death Lump",
            "Vengeance Steel",
            "Illusionary Sand",
            "Golden Sand",
            "Nightmare Sand",
            "Mobius Sand",
            "Rubbery Object",
            "Stretchy Object",
            "Unthinkable Metal",
            "Unknowable Fiber",
            "Conviction Orb",
            "Bias Crystal",
            "Order Myrrh",
            "0x5D6",
            "Nice Ornament",
            "Perpetual Edge",
            "Spark Ornament",
            "Holy Gold Lump",
            "Golden Dish",
            "Fashionable Dish",
            "Shining Dish",
            "Blessed Dish",
            "Fixed Lump",
            "Prime Steel",
            "Diamond Sheet",
            "Soul-Death Steel",
            "Tungsten",
            "Damascus",
            "Orichalcum",
            "Pyroxene Fiber",
            "Sacrifice Orb",
            "Oppression Shard",
            "Service Myrrh",
            "0x5EA",
            "Heavy Chains",
            "Strength Claw",
            "Golden Chains",
            "Platinum Hide",
            "Tyrannical Hide",
            "Riveted Hide",
            "Bloody Hide",
            "Thunder Bead",
            "Ripper Horse",
            "Sharp Thorn",
            "Steel Thorn",
            "Hard Thorn",
            "Blade Thorn",
            "Cruel Thorn",
            "Bloody Thorn",
            "Guard Cloth",
            "Thirsty Claw",
            "Activated Felt",
            "Invincible Felt",
            "0x5FE",
            "0x5FF",
            "Blank",
            "Agi",
            "Agilao",
            "Agidyne",
            "Maragi",
            "Maragion",
            "Maragidyne",
            "Garu",
            "Garula",
            "Garudyne",
            "Magaru",
            "Magarula",
            "Magarudyne",
            "Bufu",
            "Bufula",
            "Bufudyne",
            "Mabufu",
            "Mabufula",
            "Mabufudyne",
            "Zio",
            "Zionga",
            "Ziodyne",
            "Mazio",
            "Mazionga",
            "Maziodyne",
            "Megido",
            "Megidola",
            "Megidolaon",
            "Hama",
            "Hamaon",
            "Mahama",
            "Mahamaon",
            "Mudo",
            "Mudoon",
            "Mamudo",
            "Mamudoon",
            "Pulinpa",
            "Tentarafoo",
            "Evil Touch",
            "Evil Smile",
            "Ghastly Wail",
            "Balzac",
            "Valiant Dance",
            "Poisma",
            "Poison Mist",
            "Soul Break",
            "Anima Freeze",
            "Enervation",
            "Old One",
            "Galgalim Eyes",
            "Makajam",
            "Foolish Whisper",
            "Foul Breath",
            "Stagnant Air",
            "Life Drain",
            "Spirit Drain",
            "Assault Dive",
            "Sonic Punch",
            "Double Fangs",
            "Kill Rush",
            "Swift Strike",
            "Twin Shot",
            "Fatal End",
            "Mighty Swing",
            "Torrent Shot",
            "Heat Wave",
            "Gigantic Fist",
            "Blade of Fury",
            "Deathbound",
            "Arrow Rain",
            "Akasha Arts",
            "Tempest Slash",
            "Heaven's Blade",
            "Myriad Arrows",
            "God's Hand",
            "Prayala",
            "Primal Force",
            "Vorpal Blade",
            "Power Slash",
            "Gale Slash",
            "Brave Blade",
            "Herculean Strike",
            "Vicious Strike",
            "Poison Skewer",
            "Poison Arrow",
            "Blight",
            "Virus Wave",
            "Skull Cracker",
            "Mind Slice",
            "Hysterical Slap",
            "Crazy Chain",
            "Muzzle Shot",
            "Seal Bomb",
            "Arm Chopper",
            "Atom Smasher",
            "Cell Breaker",
            "Mustard Bomb",
            "Brain Shake",
            "Navas Nebula",
            "Black Spot",
            "Rainy Death",
            "Rampage",
            "Aeon Rain",
            "Agneyastra",
            "Cruel Attack",
            "Vile Assault",
            "Dia",
            "Diarama",
            "Diarahan",
            "Media",
            "Mediarama",
            "Mediarahan",
            "Salvation",
            "Amrita",
            "Recarm",
            "Samarecarm",
            "Tarunda",
            "Matarunda",
            "Sukunda",
            "Masukunda",
            "Rakunda",
            "Marakunda",
            "Dekunda",
            "Tarukaja",
            "Matarukaja",
            "Sukukaja",
            "Masukukaja",
            "Rakukaja",
            "Marakukaja",
            "Dekaja",
            "Energy Shower",
            "Debilitate",
            "Power Charge",
            "Mind Charge",
            "Tetrakarn",
            "Makarakarn",
            "Tetra Break",
            "Makara Break",
            "Tetraja",
            "Rebellion",
            "Revolution",
            "Fire Break",
            "Ice Break",
            "Wind Break",
            "Elec Break",
            "Red Wall",
            "White Wall",
            "Blue Wall",
            "Green Wall",
            "Trafuri",
            "Recarmdra",
            "Traesto",
            "Resist Physical",
            "Absorb Physical",
            "Resist Fire",
            "Absorb Fire",
            "Resist Ice",
            "Absorb Ice",
            "Resist Elec",
            "Absorb Elec",
            "Resist Wind",
            "Absorb Wind",
            "Repel Light",
            "Repel Dark",
            "Null Panic",
            "Null Exhaust",
            "Null Mute",
            "Null Fear",
            "Null Rage",
            "Null Poison",
            "Null Dizzy",
            "Null Enervate",
            "Unshaken Will",
            "Dodge Physical",
            "Evade Physical",
            "Dodge Fire",
            "Evade Fire",
            "Dodge Ice",
            "Evade Ice",
            "Dodge Wind",
            "Evade Wind",
            "Dodge Elec",
            "Evade Elec",
            "Angelic Grace",
            "Fire Boost",
            "Fire Amp",
            "Ice Boost",
            "Ice Amp",
            "Elec Boost",
            "Elec Amp",
            "Wind Boost",
            "Wind Amp",
            "Counter",
            "Counterstrike",
            "High Counter",
            "Regenerate 1",
            "Regenerate 2",
            "Regenerate 3",
            "Invigorate 1",
            "Invigorate 2",
            "Invigorate 3",
            "Growth 1",
            "Growth 2",
            "Growth 3",
            "Auto-Tarukaja",
            "Auto-Rakukaja",
            "Auto-Sukukaja",
            "Alertness",
            "Sharp Student",
            "Apt Pupil",
            "Firm Stance",
            "Spell Master",
            "Arms Master",
            "Divine Grace",
            "Endure",
            "Enduring Soul",
            "Survive Light",
            "Survive Dark",
            "Auto-Maraku",
            "Auto-Mataru",
            "Auto-Masuku",
            "Panic Boost",
            "Poison Boost",
            "Exhaust Boost",
            "Silence Boost",
            "Fear Boost",
            "Rage Boost",
            "Dizzy Boost",
            "Enervate Boost",
            "Ailment Boost",
            "Hama Boost",
            "Mudo Boost",
            "Endure Light",
            "Endure Dark",
            "Cool Breeze",
            "Victory Cry",
            "Resist Poison",
            "Resist Panic",
            "Resist Fear",
            "Resist Exhaust",
            "Resist Enervate",
            "Resist Rage",
            "Resist Dizzy",
            "Resist Mute",
            "Fast Heal",
            "Insta-Heal",
            "Patra",
            "Me Patra",
            "Re Patra",
            "Posumudi",
            "Mutudi",
            "Nervundi",
            "Heat Riser",
            "Single Shot",
            "0x6FE",
            "0x6FF",
            "Default Clothing",
            "Winter Yaso Outfit",
            "Winter Yaso Outfit",
            "Winter Yaso Outfit",
            "Winter Yaso Outfit",
            "Blank",
            "Winter Yaso Outfit",
            "Winter Yaso Outfit",
            "Teddie Costume",
            "Summer Yaso Outfit",
            "Summer Yaso Outfit",
            "Summer Yaso Outfit",
            "Summer Yaso Outfit",
            "Blank",
            "Summer Yaso Outfit",
            "Summer Yaso Outfit",
            "Blank",
            "Summer Clothes",
            "Summer Clothes",
            "Summer Clothes",
            "Summer Clothes",
            "Blank",
            "Summer Clothes",
            "Summer Clothes",
            "Summer Clothes",
            "Winter Clothes",
            "Winter Clothes",
            "Winter Clothes",
            "Winter Clothes",
            "Blank",
            "Winter Clothes",
            "Winter Clothes",
            "Blank",
            "Gekkou Uniform",
            "Gekkou Uniform",
            "Gekkou Uniform",
            "Gekkou Uniform",
            "Blank",
            "Gekkou Uniform",
            "Gekkou Uniform",
            "Gekkou Uniform",
            "Drag Costume",
            "Drag Costume",
            "Blank",
            "Blank",
            "Blank",
            "Drag Costume",
            "Blank",
            "Drag Costume",
            "Cool Trunks",
            "Summer Trunks",
            "Striped Bikini",
            "Trim Bikini",
            "Blank",
            "Dangerous Briefs",
            "School Swimsuit",
            "Sailor Trunks",
            "Yaso High Jersey",
            "Yaso High Jersey",
            "Yaso High Jersey",
            "Yaso High Jersey",
            "Blank",
            "Yaso High Jersey",
            "Yaso High Jersey",
            "Yaso High Jersey",
            "Cleaning Uniform",
            "Junes Apron",
            "Track Suit",
            "Tsukesage",
            "Blank",
            "Kingpin Duster",
            "Girls' Yaso Outfit",
            "Lord Flauntleroy",
            "Blank",
            "Blank",
            "Fighter Armor",
            "Magical Armor",
            "Blank",
            "Blank",
            "Coronet Armor",
            "Blank",
            "Neo Featherman Suit",
            "Neo Featherman Suit",
            "Neo Featherman Suit",
            "Neo Featherman Suit",
            "Blank",
            "Neo Featherman Suit",
            "Neo Featherman Suit",
            "Neo Featherman Suit",
            "Bath Towel",
            "Bath Towel",
            "Blank",
            "Blank",
            "Blank",
            "Bath Towel",
            "Blank",
            "Bath Towel",
            "Agent Suit",
            "Agent Suit",
            "Agent Suit",
            "Agent Suit",
            "Blank",
            "Agent Suit",
            "Agent Suit",
            "Agent Suit",
            "Gag Winter Outfit",
            "Gag Winter Outfit",
            "Gag Winter Outfit",
            "Gag Winter Outfit",
            "Blank",
            "Gag Winter Outfit",
            "Gag Winter Outfit",
            "Gag Glasses",
            "Gag Summer Outfit",
            "Gag Summer Outfit",
            "Gag Summer Outfit",
            "Gag Summer Outfit",
            "Blank",
            "Gag Summer Outfit",
            "Gag Summer Outfit",
            "Gag Summer Outfit",
            "Christmas Costume",
            "Christmas Costume",
            "Christmas Costume",
            "Christmas Costume",
            "Blank",
            "Christmas Costume",
            "Christmas Costume",
            "Christmas Costume",
            "Festival Jinbei",
            "Festival Jinbei",
            "Festival Yukata",
            "Festival Yukata",
            "Blank",
            "Festival Jinbei",
            "Festival Jinbei",
            "Festival Happi",
            "Halloween Costume",
            "Halloween Costume",
            "Halloween Costume",
            "Halloween Costume",
            "Blank",
            "Halloween Costume",
            "Halloween Costume",
            "Halloween Costume",
            "Midwinter Outfit",
            "Midwinter Outfit",
            "Midwinter Outfit",
            "Midwinter Outfit",
            "Blank",
            "Midwinter Outfit",
            "Midwinter Outfit",
            "Midwinter Outfit",
            "Butler Suit",
            "Butler Suit",
            "Maid Uniform",
            "Maid Uniform",
            "Blank",
            "Butler Suit",
            "Maid Uniform",
            "Butler Suit",
            "Cheer Squad Outfit",
            "Cheer Squad Outfit",
            "Cheerleader Outfit",
            "Cheerleader Outfit",
            "Blank",
            "Cheer Squad Outfit",
            "Cheerleader Outfit",
            "Cheer Squad Outfit",
            "Deep Blue Clothes",
            "Deep Blue Clothes",
            "Deep Blue Clothes",
            "Deep Blue Clothes",
            "Blank",
            "Deep Blue Clothes",
            "Deep Blue Clothes",
            "Deep Blue Clothes",
            "Hardboiled Look",
            "Junes Coveralls",
            "Kung Fu Costume",
            "Ceremonial Kimono",
            "Blank",
            "Working Clothes",
            "Detective Costume",
            "Teddie's Apron",
            "Midwinter Yaso",
            "Midwinter Yaso",
            "Midwinter Yaso",
            "Midwinter Yaso",
            "Blank",
            "Midwinter Yaso",
            "Midwinter Yaso",
            "Blank",
            "0x7C1",
            "0x7C2",
            "0x7C3",
            "0x7C4",
            "0x7C5",
            "0x7C6",
            "0x7C7",
            "0x7C8",
            "0x7C9",
            "0x7CA",
            "0x7CB",
            "0x7CC",
            "0x7CD",
            "0x7CE",
            "0x7CF",
            "0x7D0",
            "0x7D1",
            "0x7D2",
            "0x7D3",
            "0x7D4",
            "0x7D5",
            "0x7D6",
            "0x7D7",
            "0x7D8",
            "0x7D9",
            "0x7DA",
            "0x7DB",
            "0x7DC",
            "0x7DD",
            "0x7DE",
            "0x7DF",
            "0x7E0",
            "0x7E1",
            "0x7E2",
            "0x7E3",
            "0x7E4",
            "0x7E5",
            "0x7E6",
            "0x7E7",
            "0x7E8",
            "0x7E9",
            "0x7EA",
            "0x7EB",
            "0x7EC",
            "0x7ED",
            "0x7EE",
            "0x7EF",
            "0x7F0",
            "0x7F1",
            "0x7F2",
            "0x7F3",
            "0x7F4",
            "0x7F5",
            "0x7F6",
            "0x7F7",
            "Agent Set",
            "Butler Set",
            "Festival Set",
            "Cheer Squad Set",
            "Maid Set",
            "Neo Featherman Set",
            "0x7FE",
            "0x7FF",
            "Blank",
            "Unfinished Scooter",
            "Unfinished Scooter",
            "Unfinished Scooter",
            "Unfinished Scooter",
            "Unfinished Scooter",
            "Unfinished Scooter",
            "0x82F",
            "Jack Frost Doll",
            "Black Frost Doll",
            "King Frost Doll",
            "Pyro Jack Doll",
            "Neko Shogun Doll",
            "Silver Scooter",
            "Orange Scooter",
            "Light Blue Scooter",
            "Red Scooter",
            "Pink Scooter",
            "Blue Scooter",
            "0x813",
            "0x814",
            "0x815",
            "0x816",
            "0x817",
            "0x818",
            "0x819",
            "0x81A",
            "0x81B",
            "0x81C",
            "0x81D",
            "0x81E",
            "0x81F",
            "0x820",
            "0x821",
            "0x822",
            "0x823",
            "0x824",
            "0x825",
            "0x826",
            "0x827",
            "0x828",
            "Tomato Seedling",
            "Eggplant Seedling",
            "Daikon Seedling",
            "Barrier Corn",
            "Cabbage Seedling",
            "Wall Paprika",
            "Wheat Seedling",
            "Melon Seedling",
            "Tiny Soul Tomato",
            "Scapegoat Eggplant",
            "Return Daikon",
            "Tetracorn",
            "Hiranya Cabbage",
            "Red Paprika",
            "Crack Wheat",
            "Bead Melon",
            "0x839",
            "0x83A",
            "Makaracorn",
            "White Paprika",
            "Blue Paprika",
            "Green Paprika",
            "0x83F",
            "0x840",
            "0x841",
            "0x842",
            "0x843",
            "0x844",
            "0x845",
            "0x846",
            "0x847",
            "0x848",
            "0x849",
            "0x84A",
            "0x84B",
            "0x84C",
            "0x84D",
            "0x84E",
            "0x84F",
            "Iolite",
            "Blue Quartz",
            "Fluorite",
            "Smoky Quartz",
            "Lepidolite",
            "Lemon Quartz",
            "Olive Stone",
            "Howlite",
            "Milky Quartz",
            "Bloodstone",
            "Meteorite",
            "Rose Quartz",
            "Buffalo Stone",
            "Sphalerite",
            "Aurora Quartz",
            "Sunstone",
            "Angelite",
            "Rainbow Quartz",
            "Moonstone",
            "Magatsu Xandrite",
            "Kingyou Stone",
            "Gyosen Stone",
            "Yazu Stone",
            "Masuda Stone",
            "Taisui Stone",
            "Mondo Stone",
            "Takou Stone",
            "Meguro Stone",
            "Kaiou Stone",
            "Ryugu Stone",
            "0x86E",
            "0x86F",
            "0x870",
            "0x871",
            "0x872",
            "0x873",
            "0x874",
            "0x875",
            "0x876",
            "0x877",
            "0x878",
            "0x879",
            "0x87A",
            "0x87B",
            "0x87C",
            "0x87D",
            "0x87E",
            "0x87F",
            "0x880",
            "0x881",
            "0x882",
            "0x883",
            "0x884",
            "0x885",
            "0x886",
            "0x887",
            "0x888",
            "0x889",
            "0x88A",
            "0x88B",
            "0x88C",
            "0x88D",
            "0x88E",
            "0x88F",
            "0x890",
            "0x891",
            "0x892",
            "0x893",
            "0x894",
            "0x895",
            "0x896",
            "0x897",
            "0x898",
            "0x899",
            "0x89A",
            "0x89B",
            "0x89C",
            "0x89D",
            "0x89E",
            "0x89F",
            "0x8A0",
            "0x8A1",
            "0x8A2",
            "0x8A3",
            "0x8A4",
            "0x8A5",
            "0x8A6",
            "0x8A7",
            "0x8A8",
            "0x8A9",
            "0x8AA",
            "0x8AB",
            "0x8AC",
            "0x8AD",
            "0x8AE",
            "0x8AF",
            "0x8B0",
            "0x8B1",
            "0x8B2",
            "0x8B3",
            "0x8B4",
            "0x8B5",
            "0x8B6",
            "0x8B7",
            "0x8B8",
            "0x8B9",
            "0x8BA",
            "0x8BB",
            "0x8BC",
            "0x8BD",
            "0x8BE",
            "0x8BF",
            "0x8C0",
            "0x8C1",
            "0x8C2",
            "0x8C3",
            "0x8C4",
            "0x8C5",
            "0x8C6",
            "0x8C7",
            "0x8C8",
            "0x8C9",
            "0x8CA",
            "0x8CB",
            "0x8CC",
            "0x8CD",
            "0x8CE",
            "0x8CF",
            "0x8D0",
            "0x8D1",
            "0x8D2",
            "0x8D3",
            "0x8D4",
            "0x8D5",
            "0x8D6",
            "0x8D7",
            "0x8D8",
            "0x8D9",
            "0x8DA",
            "0x8DB",
            "0x8DC",
            "0x8DD",
            "0x8DE",
            "0x8DF",
            "0x8E0",
            "0x8E1",
            "0x8E2",
            "0x8E3",
            "0x8E4",
            "0x8E5",
            "0x8E6",
            "0x8E7",
            "0x8E8",
            "0x8E9",
            "0x8EA",
            "0x8EB",
            "0x8EC",
            "0x8ED",
            "0x8EE",
            "0x8EF",
            "0x8F0",
            "0x8F1",
            "0x8F2",
            "0x8F3",
            "0x8F4",
            "0x8F5",
            "0x8F6",
            "0x8F7",
            "0x8F8",
            "0x8F9",
            "0x8FA",
            "0x8FB",
            "0x8FC",
            "0x8FD",
            "0x8FE",
            "0x8FF",
            "0x900",
            "Beach Parasol",
            "Cleaning Mop",
            "Light Sword",
            "Bass",
            "Skull Rod",
            "Cheering Flag",
            "Spiked Bat",
            "Bus Stop Sign",
            "Carbon Pole",
            "Sharp Shovel",
            "Bamboo Broom",
            "0x90C",
            "0x90D",
            "0x90E",
            "0x90F",
            "0x910",
            "0x911",
            "0x912",
            "0x913",
            "0x914",
            "0x915",
            "Grilled Corn",
            "Feather Dagger",
            "Happy Maracas",
            "Pinwheel",
            "Bone",
            "Megaphone",
            "Inaba Trout",
            "Bowling Pin",
            "Chinese Cleaver",
            "0x91F",
            "0x920",
            "0x921",
            "0x922",
            "0x923",
            "0x924",
            "0x925",
            "0x926",
            "0x927",
            "0x928",
            "Flying Disc",
            "Disco Fan",
            "Feather Boomerang",
            "Tambourine",
            "Santa Fan",
            "Festival Fan",
            "Bat Saucer",
            "Table Tennis Racket",
            "Victory Fan",
            "Silver Tray",
            "Gothic Fan",
            "0x934",
            "0x935",
            "0x936",
            "0x937",
            "0x938",
            "0x939",
            "0x93A",
            "0x93B",
            "0x93C",
            "0x93D",
            "0x93E",
            "Invisible Shoes",
            "Wrestling Boots",
            "Jet Boots",
            "Inline Skates",
            "Foot-Maces",
            "Metal Geta",
            "Loose Socks",
            "Animal Slippers",
            "Spring Boots",
            "0x948",
            "0x949",
            "0x94A",
            "0x94B",
            "0x94C",
            "0x94D",
            "0x94E",
            "0x94F",
            "0x950",
            "Bodyboard",
            "Factory Sign",
            "Feather Shield",
            "0x954",
            "Big Cymbal",
            "Christmas Wreath",
            "Casket Lid",
            "Colored Bench",
            "Flat Taiko",
            "Guardian",
            "Dining Table",
            "Half-Sized Tatami",
            "0x95D",
            "0x95E",
            "0x95F",
            "0x960",
            "0x961",
            "0x962",
            "0x963",
            "0x964",
            "0x965",
            "0x966",
            "Water Gun",
            "Feather Shot",
            "Bat Magnum",
            "Flintlock",
            "Frost Shot",
            "Special Ray Gun",
            "Rubber Band Gun",
            "Machine Pistol",
            "0x96F",
            "0x970",
            "0x971",
            "0x972",
            "0x973",
            "0x974",
            "0x975",
            "0x976",
            "0x977",
            "0x978",
            "Beach Ball",
            "Animal Paw",
            "Feather Claw",
            "Reindeer Hoof",
            "Pyro Puppet",
            "Hyper Drill",
            "Crab Claw",
            "Frost Puppet",
            "0x981",
            "Strange Sword",
            "Strange Daggers",
            "Strange Greaves",
            "Strange Fan",
            "Strange Shield",
            "Strange Firearm",
            "Strange Claw",
            "0x989",
            "0x98A",
            "0x98B",
            "0x98C",
            "0x98D",
            "0x98E",
            "0x98F",
            "0x990",
            "0x991",
            "0x992",
            "0x993",
            "0x994",
            "0x995",
            "0x996",
            "0x997",
            "0x998",
            "0x999",
            "0x99A",
            "0x99B",
            "0x99C",
            "0x99D",
            "0x99E",
            "0x99F",
            "0x9A0",
            "0x9A1",
            "0x9A2",
            "0x9A3",
            "0x9A4",
            "0x9A5",
            "0x9A6",
            "0x9A7",
            "0x9A8",
            "0x9A9",
            "0x9AA",
            "0x9AB",
            "0x9AC",
            "0x9AD",
            "0x9AE",
            "0x9AF",
            "0x9B0",
            "0x9B1",
            "0x9B2",
            "0x9B3",
            "0x9B4",
            "0x9B5",
            "0x9B6",
            "0x9B7",
            "0x9B8",
            "0x9B9",
            "0x9BA",
            "0x9BB",
            "0x9BC",
            "0x9BD",
            "0x9BE",
            "0x9BF",
            "0x9C0",
            "0x9C1",
            "0x9C2",
            "0x9C3",
            "0x9C4",
            "0x9C5",
            "0x9C6",
            "0x9C7",
            "0x9C8",
            "0x9C9",
            "0x9CA",
            "0x9CB",
            "0x9CC",
            "0x9CD",
            "0x9CE",
            "0x9CF",
            "0x9D0",
            "0x9D1",
            "0x9D2",
            "0x9D3",
            "0x9D4",
            "0x9D5",
            "0x9D6",
            "0x9D7",
            "0x9D8",
            "0x9D9",
            "0x9DA",
            "0x9DB",
            "0x9DC",
            "0x9DD",
            "0x9DE",
            "0x9DF",
            "0x9E0",
            "0x9E1",
            "0x9E2",
            "0x9E3",
            "0x9E4",
            "0x9E5",
            "0x9E6",
            "0x9E7",
            "0x9E8",
            "0x9E9",
            "0x9EA",
            "0x9EB",
            "0x9EC",
            "0x9ED",
            "0x9EE",
            "0x9EF",
            "0x9F0",
            "0x9F1",
            "0x9F2",
            "0x9F3",
            "0x9F4",
            "0x9F5",
            "0x9F6",
            "0x9F7",
            "0x9F8",
            "0x9F9",
            "0x9FA",
            "0x9FB",
            "0x9FC",
            "0x9FD",
            "0x9FE",
            "0x9FF",
            };

            #endregion
    }
}
