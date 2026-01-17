namespace Team9Game.Scripts.Combat;

public static class AttackCalculator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <param name="type"></param>
    /// <param name="defense"></param>
    /// <returns></returns>
    public static double OnHitDamage(double baseDamage, AttackType type, double defense)
    {
        return 0;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <param name="type"></param>
    /// <param name="defense"></param>
    /// <returns></returns>
    public static double OffHitDamage(double baseDamage, AttackType type, double defense)
    {
        return baseDamage * (100 - defense) / 100;
    }
}