using System;
using Timberborn.Modding;
using Timberborn.SettingsSystem;
namespace ModSettings.Core {
[Flags] public enum ModSettingsContext { None=0,MainMenu=1,Game=2,MapEditor=4,All=7 }
public sealed class ModSettingDescriptor {
 public static ModSettingDescriptor Create(string name)=>new ModSettingDescriptor();
 public ModSettingDescriptor SetTooltip(string tooltip)=>this;
 public ModSettingDescriptor SetEnableCondition(Func<bool> isEnabled)=>this;
}
public abstract class ModSetting {
 protected ModSetting(ModSettingDescriptor descriptor) {}
 public abstract void Reset();
}
public class ModSetting<T> : ModSetting {
 public ModSetting(T value, ModSettingDescriptor descriptor):base(descriptor) { DefaultValue=value; }
 public T DefaultValue {get;}
 public T Value {get; private set;}=default!;
 public ModSettingDescriptor Descriptor {get;}=new ModSettingDescriptor();
 public override void Reset() {}
}
public sealed class ModSettingsOwnerRegistry {}
public abstract class ModSettingsOwner {
 protected ModSettingsOwner(ISettings settings,ModSettingsOwnerRegistry registry,ModRepository repo) {}
 protected abstract string ModId {get;}
 public virtual ModSettingsContext ChangeableOn => ModSettingsContext.MainMenu;
}
}