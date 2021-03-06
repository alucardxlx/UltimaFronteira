using System;
using Server.Targeting;

namespace Server.Items
{
    public class ExoticToolkit : BaseDecayingItem
    {
        [Constructable]
        public ExoticToolkit() 
            : base(0x1EB9)
        {
            Name = "Kit Ferramentas Goblin";
            this.Hue = 2500;
            this.Weight = 1;
        }

        public override int Lifespan { get { return 604800; } }
        public override bool UseSeconds { get { return false; } }

        public override int LabelNumber { get { return 1153866; } } // Exotic Toolkit

        public ExoticToolkit(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(500325); // I am too far away to do that.
            else
            {
                from.Target = new InternalTarget(this);
                from.SendMessage("Use isto em um nexus quebrado"); // Use this on a broken Nexus.
            }
        }

        public class InternalTarget : Target
        {
            public ExoticToolkit m_Toolkit;
            public InternalTarget(ExoticToolkit toolkit) : base(-1, true, TargetFlags.None)
            {
                m_Toolkit = toolkit;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is NexusComponent2)
                {
                    NexusComponent2 addon = ((NexusComponent2)targeted) as NexusComponent2;

                    if (addon.Addon is ExodusNexus)
                    {
                        if (!((ExodusNexus)addon.Addon).Active)
                        {
                            ((ExodusNexus)addon.Addon).OpenGump(from);
                        }
                    }                        
                }
            }
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
