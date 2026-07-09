namespace Warzone.Combat
{
    public static class DamageModifierRule
    {
        public static int Apply(int baseDamage, float damageMultiplier)
        {
            if (baseDamage <= 0)
            {
                return 0;
            }

            int damage = (int)System.Math.Round(baseDamage * damageMultiplier, System.MidpointRounding.AwayFromZero);
            return damage < 1 ? 1 : damage;
        }
    }
}
