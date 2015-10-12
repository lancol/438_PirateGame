using Microsoft.Xna.Framework.Graphics;
namespace PirateGame
{
    class NPCShip : Ship
    {
        string faction;
        string stance;

        public NPCShip(int x, int y, float rotate, string Faction)
        {
            setX(x);
            setY(y);
            setRotate(rotate);
            //defaults
            setAttack(1);
            setDefense(1);
            setHealth(10);
            setGold(100);
            faction = Faction;
            stance = "passive";
        }

        public void setFaction(string Faction)
        {
            faction = Faction;
        }

        public void setStance(string Stance)
        {
            stance = Stance;
        }

        public string getFaction()
        {
            return faction;
        }

        public string getStance()
        {
            return stance;
        }
    }
}
