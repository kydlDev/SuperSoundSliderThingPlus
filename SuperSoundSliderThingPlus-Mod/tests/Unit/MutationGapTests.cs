using SuperSoundSliderThingPlus.Features.Sound.Domain;

namespace SuperSoundSliderThingPlus.Tests.Unit
{
    /// Pins down the exact limits of the sound rules (Stryker Advanced, 2026-11-16): the value ON the limit must behave like the code says.
    public class MutationGapTests
    {
        [Test] public void FloatCompare_ADifferenceOfExactlyTheToleranceIsNotTheSame()
        {
            Assert.That(FloatCompare.Same(0f, FloatCompare.Tolerance), Is.False);
            Assert.That(FloatCompare.Same(0f, FloatCompare.Tolerance / 2f), Is.True);
        }

        [Test] public void MasterVolume_SameMute_ADifferenceOfExactlyTheLimitIsNotTheSame()
        {
            Assert.That(MasterVolumeRules.SameMute(0f, MasterVolumeRules.SameWithin), Is.False);
            Assert.That(MasterVolumeRules.SameMute(0f, MasterVolumeRules.SameWithin / 2f), Is.True);
        }

        [Test] public void MasterVolume_InRangeValues_SayInRange()
        {
            Assert.That(MasterVolumeRules.MuteFromVolume(0.5f).Reason, Is.EqualTo(MasterVolumeRules.InRange));
            Assert.That(MasterVolumeRules.VolumeFromMute(50f).Reason, Is.EqualTo(MasterVolumeRules.InRange));
        }

        [TestCase(150f)]
        [TestCase(-5f)]
        public void MasterVolume_OutOfRangeMute_SaysClamped(float mute)
        {
            Assert.That(MasterVolumeRules.VolumeFromMute(mute).Reason, Is.EqualTo(MasterVolumeRules.Clamped));
        }

        [Test] public void LoudnessEstimate_ABlockExactlyAtTheSilenceLimitIsCounted()
        {
            var e = new LoudnessEstimate();

            Assert.That(e.Add(LoudnessRules.SilenceBelow), Is.True);
            Assert.That(e.Blocks, Is.EqualTo(1));
        }

        [Test] public void Gain_EqualMinAndMax_IsAValidRange()
        {
            var r = LoudnessRules.Gain(0.1f, 0.5f, 2f, 2f);

            Assert.That(r.Value, Is.EqualTo(2f));
            Assert.That(r.Reason, Is.EqualTo(LoudnessRules.Limited));
        }

        [Test] public void Gain_AMeasurementExactlyAtTheSilenceLimitIsMeasured()
        {
            var r = LoudnessRules.Gain(LoudnessRules.SilenceBelow, 0.1f, 0.1f, 10f);

            Assert.That(r.Value, Is.EqualTo(10f));
            Assert.That(r.Reason, Is.EqualTo(LoudnessRules.Limited));
        }

        [TestCase("sfx_thunder(clone)", "")]
        [TestCase("sfx_thunder", "Hen(clone)")]
        public void Classifier_LowerCaseCloneSuffix_IsDropped(string sound, string owner)
        {
            var expected = owner.Length == 0 ? SoundCategory.Weather : SoundCategory.Weather;

            Assert.That(SoundClassifier.Classify(sound, owner), Is.EqualTo(expected));
            Assert.That(SoundClassifier.Classify("sfx_love", "Hen(clone)"), Is.EqualTo(SoundCategory.ChickenLove));
        }
    }
}
