using SuperSoundSliderThingPlus.Features.Sound.Domain;

namespace SuperSoundSliderThingPlus.Tests.Unit;

/// Which [General] Enable switch gates which slider (user 2026-11-17): Master / Music / Sound Effects = Game menu sounds, every other slider = Special adjustments.
internal class SoundGroupRulesTests
{
    [TestCase(SoundSlider.Master)]
    [TestCase(SoundSlider.Music)]
    [TestCase(SoundSlider.Effects)]
    public void MenuSliders_AreTheGameMenuSoundsGroup(SoundSlider slider)
    {
        Assert.That(SoundMixRules.IsMenuSlider(slider), Is.True);
    }

    [TestCase(SoundSlider.Weather)]
    [TestCase(SoundSlider.WolfPup)]
    [TestCase(SoundSlider.ShieldGenerator)]
    [TestCase(SoundSlider.ChickenLove)]
    [TestCase(SoundSlider.Portal)]
    public void OtherSliders_AreTheSpecialAdjustmentsGroup(SoundSlider slider)
    {
        Assert.That(SoundMixRules.IsMenuSlider(slider), Is.False);
    }

    [TestCase(SoundSlider.Master, true, false, true)]
    [TestCase(SoundSlider.Master, false, true, false)]
    [TestCase(SoundSlider.Music, true, false, true)]
    [TestCase(SoundSlider.Effects, false, true, false)]
    [TestCase(SoundSlider.Portal, false, true, true)]
    [TestCase(SoundSlider.Portal, true, false, false)]
    [TestCase(SoundSlider.Weather, true, false, false)]
    [TestCase(SoundSlider.ChickenLove, false, true, true)]
    public void GroupOn_FollowsTheSwitchOfTheSlidersGroup(SoundSlider slider, bool menuOn, bool specialOn, bool expected)
    {
        Assert.That(SoundMixRules.GroupOn(slider, menuOn, specialOn), Is.EqualTo(expected));
    }
}
