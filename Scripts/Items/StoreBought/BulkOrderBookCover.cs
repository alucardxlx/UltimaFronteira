using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Engines.BulkOrders;

namespace Server.Items
{
    public enum CoverType
    {
        Normal,
        DullCopper,
        ShadowIron,
        Copper,
        Bronze,
        Gold,
        Agapite,
        Verite,
        Valorite,
        Spined,
        Horned,
        Barbed,
        Oak,
        Ash,
        Yew,
        Heartwood,
        Bloodwood,
        Frostwood,
        Alchemy,
        Blacksmith, 
        Cooking,
        Fletching,
        Carpentry,
        Inscription,
        Tailoring,
        Tinkering
    }

    public class CoverInfo
    {
        public static List<CoverInfo> Infos { get; private set; }

        public static void Initialize()
        {
            Infos = new List<CoverInfo>();
 
            Infos.Add(new CoverInfo(CoverType.Normal, 1071097, 0));
            Infos.Add(new CoverInfo(CoverType.DullCopper, 1071101, CraftResources.GetHue(CraftResource.Berilo)));
            Infos.Add(new CoverInfo(CoverType.ShadowIron, 1071107, CraftResources.GetHue(CraftResource.Vibranium)));
            Infos.Add(new CoverInfo(CoverType.Copper, 1071108, CraftResources.GetHue(CraftResource.Cobre)));
            Infos.Add(new CoverInfo(CoverType.Bronze, 1071109, CraftResources.GetHue(CraftResource.Bronze)));
            Infos.Add(new CoverInfo(CoverType.Gold, 1071112, CraftResources.GetHue(CraftResource.Dourado)));
            Infos.Add(new CoverInfo(CoverType.Agapite, 1071113, CraftResources.GetHue(CraftResource.Niobio)));
            Infos.Add(new CoverInfo(CoverType.Verite, 1071114, CraftResources.GetHue(CraftResource.Lazurita)));
            Infos.Add(new CoverInfo(CoverType.Valorite, 1071115, CraftResources.GetHue(CraftResource.Quartzo)));
            Infos.Add(new CoverInfo(CoverType.Spined, 1071098, CraftResources.GetHue(CraftResource.CouroSpinned)));
            Infos.Add(new CoverInfo(CoverType.Horned, 1071099, CraftResources.GetHue(CraftResource.CouroHorned)));
            Infos.Add(new CoverInfo(CoverType.Barbed, 1071100, CraftResources.GetHue(CraftResource.CouroBarbed)));
            Infos.Add(new CoverInfo(CoverType.Oak, 1071410, CraftResources.GetHue(CraftResource.Carvalho)));
            Infos.Add(new CoverInfo(CoverType.Ash, 1071411, CraftResources.GetHue(CraftResource.Pinho)));
            Infos.Add(new CoverInfo(CoverType.Yew, 1071412, CraftResources.GetHue(CraftResource.Mogno)));
            Infos.Add(new CoverInfo(CoverType.Heartwood, 1071413, CraftResources.GetHue(CraftResource.Eucalipto)));
            Infos.Add(new CoverInfo(CoverType.Bloodwood, 1071414, CraftResources.GetHue(CraftResource.Carmesim)));
            Infos.Add(new CoverInfo(CoverType.Frostwood, 1071415, CraftResources.GetHue(CraftResource.Gelo)));
            Infos.Add(new CoverInfo(CoverType.Alchemy, 1157605, 2505, 1002000));
            Infos.Add(new CoverInfo(CoverType.Blacksmith, 1157605, 0x44E, 1157607));
            Infos.Add(new CoverInfo(CoverType.Cooking, 1157605, 1169, 1002063));
            Infos.Add(new CoverInfo(CoverType.Fletching, 1157605, 1425, 1002047));
            Infos.Add(new CoverInfo(CoverType.Carpentry, 1157605, 1512, 1002054));
            Infos.Add(new CoverInfo(CoverType.Inscription, 1157605, 2598, 1002090));
            Infos.Add(new CoverInfo(CoverType.Tailoring, 1157605, 0x483, 1002155));
            Infos.Add(new CoverInfo(CoverType.Tinkering, 1157605, 1109, 1002162));
        }

        public CoverType Type { get; private set; }
        public TextDefinition Label { get; private set; }
        public int Hue { get; private set; }
        public TextDefinition Args { get; private set; }

        public CoverInfo(CoverType type, TextDefinition label, int hue, TextDefinition args = null)
        {
            Type = type;
            Label = label;
            Hue = hue;
            Args = args;
        }
    }

    public class BulkOrderBookCover : Item
    {
        private CoverType _CoverType;
        private int _UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public CoverType CoverType
        {
            get { return _CoverType; }
            set
            {
                var current = _CoverType;

                if (current != value)
                {
                    _CoverType = value;
                    InvalidateHue();
                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return _UsesRemaining; } set { _UsesRemaining = value; InvalidateProperties(); } }

        [Constructable]
        public BulkOrderBookCover(CoverType type)
            : this(type, 30)
        {
        }

        [Constructable]
        public BulkOrderBookCover(CoverType type, int uses)
            : base(0x2831)
        {
            UsesRemaining = uses;

            LootType = LootType.Blessed;
            CoverType = type;
        }

        public BulkOrderBookCover(Serial serial)
            : base(serial)
        {
        }

        public void InvalidateHue()
        {
            var info = CoverInfo.Infos.FirstOrDefault(x => x.Type == _CoverType);

            if (info != null)
            {
                Hue = info.Hue;
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            var info = CoverInfo.Infos.FirstOrDefault(x => x.Type == _CoverType);

            if (info != null)
            {
                if (info.Args != null)
                {
                    list.Add(1157605, info.Args.ToString()); // Bulk Order Cover (~1_HUE~)
                }
                else if (info.Label.Number > 0)
                {
                    list.Add(info.Label.Number);
                }
                else
                {
                    list.Add(1114057, info.Label.ToString()); // ~1_val~
                }
            }
            else
            {
                list.Add(1071097); // Bulk Order Cover (Normal)
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, _UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1071121); // Select the bulk order book you want to replace a cover.
                from.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (m, targeted) =>
                    {
                        if (targeted is BulkOrderBook)
                        {
                            var bob = (BulkOrderBook)targeted;

                            if (bob.IsChildOf(m.Backpack))
                            {
                                if (bob.Hue != Hue)
                                {
                                    bob.Hue = Hue;
                                    UsesRemaining--;

                                    m.SendLocalizedMessage(1071119); // You have successfully given the bulk order book a new cover.
                                    m.PlaySound(0x048);

                                    if (UsesRemaining <= 0)
                                    {
                                        m.SendLocalizedMessage(1071120); // You have used up all the bulk order book covers.
                                        Delete();
                                    }
                                }
                                else
                                {
                                    m.SendLocalizedMessage(1071122); // You cannot cover it with same color.
                                }
                            }
                            else
                            {
                                m.SendLocalizedMessage(1071117); // You cannot use this item for it.
                            }
                        }
                        else
                        {
                            m.SendLocalizedMessage(1071118); // You can only cover a bulk order book with this item.
                        }
                    });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.Write(_UsesRemaining);
            writer.Write((int)_CoverType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _UsesRemaining = reader.ReadInt();
                    _CoverType = (CoverType)reader.ReadInt();
                    break;
                case 0:
                    _UsesRemaining = 30;
                    break;
            }
        }
    }

    public class BagOfBulkOrderCovers : Bag
    {
        public override int LabelNumber { get { return 1071116; } } // Bag of bulk order covers

        public BagOfBulkOrderCovers(int start, int end)
        {
            for(int i = start; i <= end; i++)
            {
                if (i >= 0 && i < CoverInfo.Infos.Count)
                {
                    DropItem(new BulkOrderBookCover(CoverInfo.Infos[i].Type));
                }
            }
        }

        public BagOfBulkOrderCovers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
