using System;
using System.Collections.Generic;

namespace Zenbo.BotService.Contracts
{
    [Serializable]
    public class RobotFeedback
    {
        /// <summary>
        /// From https://zenbo.asus.com/developer/documents/Design-Guideline/Zenbo-Introduction/Emotions
        /// </summary>
        internal static class Emotions
        {
            public static string Active => @"active";
            public static string AwareLeft => @"aware_L";
            public static string AwareRight => @"aware_R";
            public static string Confident => @"confident";
            public static string Default => @"default";
            public static string DefaultStill => @"default_still";
            public static string Doubting => @"doubting";
            public static string Expecting => @"expecting";
            public static string Happy => @"happy";
            public static string Helpless => @"helpless";
            public static string Impatient => @"impatient";
            public static string Innocent => @"innocent";
            public static string Interested => @"interested";
            public static string Lazy => @"lazy";
            public static string Pleased => @"pleased";
            public static string Pretending => @"pretending";
            public static string Proud => @"proud";
            public static string Questioning => @"questioning";
            public static string Serious => @"serious";
            public static string Shocked => @"shocked";
            public static string Shy => @"shy";
            public static string Tired => @"tired";
            public static string Worried => @"worried";
        }

        public IList<string> Emotion { get; set; } = new[] { Emotions.Default };

        /// <summary>
        /// From https://zenbo.asus.com/developer/documents/Design-Guideline/Zenbo-Introduction/Emotions
        /// </summary>
        internal static class Motions
        {
            public static string Default1 => @"0";
            public static string Default2 => @"1";
            public static string Nod1 => @"2";
            public static string HeadUpQuick => @"3";
            public static string HeadUp => @"4";
            public static string ShakeHead => @"5";
            public static string HeadUpSlow => @"6";
            public static string HeadUpright => @"7";
            public static string HeadDown1 => @"8";
            public static string HeadDownward1 => @"9";
            public static string HeadDownward2 => @"10";
            public static string ShakeHeadToCenter => @"11";
            public static string HeadDownward3 => @"12";
            public static string HeadUpSlow2 => @"13";
            public static string HeadDownward4 => @"14";
            public static string DanceFastWithBase => @"15";
            public static string HeadUpQuick2 => @"16";
            public static string MusicWithBase => @"17";

            public static string TurnLeftSlow => @"18";
            public static string TurnLeftFast => @"19";
            public static string ShakeHeadNo => @"20";
            public static string DanceFastNeckOnly => @"21";
            public static string BodyTwist => @"22";
            public static string BodyTwistWithNeckDown => @"23";
            public static string DanceSlowWithBase => @"24";
            public static string ShakeHeadDance => @"25";
            public static string HeadTwist => @"26";
            public static string DanceVeryFast => @"27";
            public static string ShakeHeadWhereSound => @"28";
            public static string HeadQuickDownward => @"42";
            public static string HeadDownward5 => @"43";
            public static string TurnRightSlow => @"44";
            public static string TurnRightFast => @"45";
            public static string ReturnAndTurnLeftWithBase => @"46";
            public static string ReturnAndTurnRightWithBase => @"47";
            public static string ReturnAndTurnLeftNeckOnly => @"48";
            public static string ReturnAndTurnRightNeckOnly => @"49";
            public static string HeadUp2 => @"54";
            public static string FindFace => @"1007";
            public static string Still => string.Empty;
        }

        public IList<string> Motion { get; set; } = new[] { Motions.Default1 };

        public string SpokenText { get; set; }
        public string SpokenSSML { get; set; }
    }
}