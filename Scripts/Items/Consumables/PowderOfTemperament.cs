using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class PowderOfTemperament : Item, IUsesRemaining
    {
        public static bool CanPOFJewelry = Config.Get("Loot.CanPOFJewelry", false);

        private int m_UsesRemaining;

        [Constructable]
        public PowderOfTemperament()
            : this(10)
        {
        }

        [Constructable]
        public PowderOfTemperament(int charges)
            : base(4102)
        {
            Name = "Po de refinamento";
            Weight = 1.0;
            Hue = 2419;
            UsesRemaining = charges;
        }

        public PowderOfTemperament(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }
        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1049082;
            }
        }// powder of fortifying
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo(Mobile m)
        {
            LabelToAffix(m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString()); // Durability
        }

        public override void OnSingleClick(Mobile from)
        {
            DisplayDurabilityTo(from);

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        private class InternalTarget : Target
        {
            private readonly PowderOfTemperament m_Powder;
            public InternalTarget(PowderOfTemperament powder)
                : base(2, false, TargetFlags.None)
            {
                m_Powder = powder;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Powder.Deleted || m_Powder.UsesRemaining <= 0)
                {
                    from.SendMessage("Acabou..."); // You have used up your powder of temperament.
                    return;
                }

                if (targeted is Item)
                {
                    Item item = (Item)targeted;
                    bool noGo = false;
                    int antique = 0;

                    if (!Server.Engines.Craft.Repair.AllowsRepair(item, null) || (item is BaseJewel && !CanPOFJewelry))
                    {
                        from.SendMessage("Apenas pode usar em items que tenham durabilidade"); // You cannot use the powder on that item.
                        return;
                    }

                    #region SA
                    /*
                    if (item is BaseWeapon)
                    {
                        if (((BaseWeapon)item).Attributes.Brittle > 0 || ((BaseWeapon)item).NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = ((BaseWeapon)item).NegativeAttributes.Antique;
                    }
                    else if (item is BaseArmor)
                    {
                        if (((BaseArmor)item).Attributes.Brittle > 0 || ((BaseArmor)item).NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = ((BaseArmor)item).NegativeAttributes.Antique;
                    }
                    else if (item is BaseClothing)
                    {
                        if (((BaseClothing)item).Attributes.Brittle > 0 || ((BaseClothing)item).NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = ((BaseClothing)item).NegativeAttributes.Antique;
                    }
                    else if (item is BaseJewel)
                    {
                        if (((BaseJewel)item).Attributes.Brittle > 0 || ((BaseJewel)item).NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = ((BaseJewel)item).NegativeAttributes.Antique;
                    }
                    else if (item is BaseTalisman && ((BaseTalisman)item).Attributes.Brittle > 0)
                    {
                        noGo = true;
                    }
                    if (noGo)
                    {
                        from.SendLocalizedMessage(1149799); //That cannot be used on brittle items.
                        return;
                    }
                    */
                    #endregion

                    if (targeted is IDurability)
                    {
                        IDurability wearable = (IDurability)targeted;

                        if (!wearable.CanFortify)
                        {
                            from.SendMessage("Apenas pode usar em items que tenham durabilidade"); // You cannot use the powder on that item.
                            return;
                        }

                        if ((item.IsChildOf(from.Backpack) || (Core.ML && item.Parent == from)) && m_Powder.IsChildOf(from.Backpack))
                        {
                            int origMaxHP = wearable.MaxHitPoints;
                            int origCurHP = wearable.HitPoints;

                            if (origMaxHP > 0)
                            {
                                int initMaxHP = antique <= 0 ? 255 : antique == 1 ? 250 : antique == 2 ? 200 : 150;

                                wearable.UnscaleDurability();

                                if (wearable.MaxHitPoints < initMaxHP)
                                {
                                    if (antique > 0)
                                    {
                                        wearable.MaxHitPoints = initMaxHP;
                                        wearable.HitPoints += initMaxHP;
                                    }
                                    else
                                    {
                                        int bonus = initMaxHP - wearable.MaxHitPoints;

                                        if (bonus > 10)
                                            bonus = 10;

                                        wearable.MaxHitPoints += bonus;
                                        wearable.HitPoints += bonus;
                                    }

                                    wearable.ScaleDurability();

                                    if (wearable.MaxHitPoints > 255)
                                        wearable.MaxHitPoints = 255;
                                    if (wearable.HitPoints > 255)
                                        wearable.HitPoints = 255;

                                    if (wearable.MaxHitPoints > origMaxHP)
                                    {
                                        from.PlayAttackAnimation();
                                        from.SendMessage("Voce aplicou o pozinho de refinamento no item"); // You successfully use the powder on the item.
                                        from.PlaySound(0x247);

                                        --m_Powder.UsesRemaining;

                                        if (m_Powder.UsesRemaining <= 0)
                                        {
                                            from.SendMessage("Seu po de refinamento acabou"); // You have used up your powder of fortifying.
                                            m_Powder.Delete();
                                        }

                                        if (antique > 0)
                                        {
                                            if (item is BaseWeapon) ((BaseWeapon)item).NegativeAttributes.Antique++;
                                            if (item is BaseArmor) ((BaseArmor)item).NegativeAttributes.Antique++;
                                            if (item is BaseJewel) ((BaseJewel)item).NegativeAttributes.Antique++;
                                            if (item is BaseClothing) ((BaseClothing)item).NegativeAttributes.Antique++;
                                        }
                                    }
                                    else
                                    {
                                        wearable.MaxHitPoints = origMaxHP;
                                        wearable.HitPoints = origCurHP;
                                        from.SendMessage("O item nao pode ser melhorado"); // The item cannot be improved any further.
                                    }
                                }
                                else
                                {
                                    from.SendMessage("O item nao pode ser melhorado");  // The item cannot be improved any further.
                                    wearable.ScaleDurability();
                                }
                            }
                            else
                            {
                                from.SendMessage("Nao posso usar nisso"); // You cannot use the powder on that item.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1049083); // You cannot use the powder on that item.
                }
            }
        }
    }
}
