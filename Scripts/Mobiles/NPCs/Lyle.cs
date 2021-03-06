using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class ThePenIsMightierQuest : BaseQuest
    { 
        public ThePenIsMightierQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(RecallScroll), "recall scroll", 5, 0x1F4C));
						
            this.AddReward(new BaseReward(typeof(RedLeatherBook), 1075545)); // a book bound in red leather
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(3);
            }
        }
        /* The Pen is Mightier */
        public override object Title
        {
            get
            {
                return 1075542;
            }
        }
        /* Do you know anything about 'Inscription?' I've been trying to get my hands on some hand crafted Recall 
        scrolls for a while now, and I could really use some help. I don't have a scribe's pen, let alone a 
        spellbook with Recall in it, or blank scrolls, so there's no way I can do it on my own. How about you 
        though? I could trade you one of my old leather bound books for some. */
        public override object Description
        {
            get
            {
                return "Voce sabe algo sobre inscription ? Bom eu nao sei, e preciso de pergaminhos de recall para minhas jornadas. Poderia me conseguir alguns ?";
            }
        }
        /* Hmm, thought I had your interest there for a moment. It's not everyday you see a book made from real 
        daemon skin, after all! */
        public override object Refuse
        {
            get
            {
                return "Awn achei que voce ajudaria.";
            }
        }
        /* Inscribing... yes, you'll need a scribe's pen, some reagents, some blank scroll, and of course your own 
        magery book. You might want to visit the magery shop if you're lacking some materials. */
        public override object Uncomplete
        {
            get
            {
                return "Voce precisara ir a loja do escriba para conseguir escrever tal pergaminho... Ou compra-los!";
            }
        }
        /* Ha! Finally! I've had a rune to the waterfalls near Justice Isle that I've been wanting to use for the 
        longest time, and now I can visit at last. Here's that book I promised you... glad to be rid of it, to be 
        honest. */
        public override object Complete
        {
            get
            {
                return "Yay finalmente, muito obrigado !";
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

    public class Lyle : MondainQuester
    {
        [Constructable]
        public Lyle()
            : base("Lyle", "the mage")
        { 
        }

        public Lyle(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ThePenIsMightierQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83F7;
            this.HairItemID = 0x204A;
            this.HairHue = 0x459;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());		
            this.AddItem(new ThighBoots());
            this.AddItem(new Robe(0x2FD));
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

    public class RedLeatherBook : BaseBook
    {
        [Constructable]
        public RedLeatherBook()
            : base(0xFF2)
        {
            this.Hue = 0x485;
        }

        public RedLeatherBook(Serial serial)
            : base(serial)
        {
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
