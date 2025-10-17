// อย่าให้สืบจาก MonoBehaviour
using System.Collections.Generic;
using UnityEngine;

public sealed class ItemRuntime
{
    public ItemSO Def { get; }                 // definition (asset)
    private readonly object sourceTag;         // คีย์สำหรับลบ modifiers ยกชุด
    private readonly List<StatsType> touched = new();

    public ItemRuntime(ItemSO def)             // <— constructor ที่หายไป
    {
        Def = def ?? throw new System.ArgumentNullException(nameof(def));
        sourceTag = this;
    }

    public void Equip(Player p)
    {
        touched.Clear();
        foreach (var spec in Def.modifiers)
        {
            var st = p.GetStat(spec.stat);     // extension GetStat(...) ที่ให้ไว้ก่อนหน้า
            int order = spec.orderOverride >= 0 ? spec.orderOverride : (int)spec.mode;

            var mod = new StatModifier(spec.value, spec.mode, order, sourceTag);
            st.AddModifier(mod);
            touched.Add(spec.stat);
        }

        if (touched.Contains(StatsType.MaxHealth))
            p.currentHealth = Mathf.Min(p.currentHealth, p.MaxHealth.Value);
    }

    public void Unequip(Player p)
    {
        foreach (var s in touched)
            p.GetStat(s).RemoveAllModifiersFromSource(sourceTag);

        touched.Clear();
        p.currentHealth = Mathf.Min(p.currentHealth, p.MaxHealth.Value);
    }
}
