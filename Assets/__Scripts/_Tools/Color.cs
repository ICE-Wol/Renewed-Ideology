using UnityEngine;

/// <summary>
/// 命名颜色调色板，返回 UnityEngine.Color。避免与 UnityEngine.Color 类型冲突。
/// </summary>
public static class ColorPalette
{
    static Color RGB(byte r, byte g, byte b)
        => new Color(r / 255f, g / 255f, b / 255f);

    // Reds
    public static Color indianRed => RGB(205, 92, 92);
    public static Color lightCoral => RGB(240, 128, 128);
    public static Color salmon => RGB(250, 128, 114);
    public static Color darkSalmon => RGB(233, 150, 122);
    public static Color lightSalmon => RGB(255, 160, 122);
    public static Color crimson => RGB(220, 20, 60);
    public static Color fireBrick => RGB(178, 34, 34);
    public static Color darkRed => RGB(139, 0, 0);

    // Pinks
    public static Color pink => RGB(255, 192, 203);
    public static Color lightPink => RGB(255, 182, 193);
    public static Color hotPink => RGB(255, 105, 180);
    public static Color deepPink => RGB(255, 20, 147);
    public static Color mediumVioletRed => RGB(199, 21, 133);
    public static Color paleVioletRed => RGB(219, 112, 147);

    // Oranges
    public static Color coral => RGB(255, 127, 80);
    public static Color tomato => RGB(255, 99, 71);
    public static Color orangeRed => RGB(255, 69, 0);
    public static Color darkOrange => RGB(255, 140, 0);
    public static Color orange => RGB(255, 165, 0);

    // Yellows
    public static Color gold => RGB(255, 215, 0);
    public static Color lightYellow => RGB(255, 255, 224);
    public static Color yellowGreen => RGB(154, 205, 50);
    public static Color lemonChiffon => RGB(255, 250, 205);
    public static Color lightGoldenRodYellow => RGB(250, 250, 210);
    public static Color papayaWhip => RGB(255, 239, 213);
    public static Color moccasin => RGB(255, 228, 181);
    public static Color peachPuff => RGB(255, 218, 185);
    public static Color paleGoldenRod => RGB(238, 232, 170);
    public static Color khaki => RGB(240, 230, 140);
    public static Color darkKhaki => RGB(189, 183, 107);

    // Greens
    public static Color lawnGreen => RGB(124, 252, 0);
    public static Color chartreuse => RGB(127, 255, 0);
    public static Color limeGreen => RGB(50, 205, 50);
    public static Color forestGreen => RGB(34, 139, 34);
    public static Color seaGreen => RGB(46, 139, 87);
    public static Color mediumSeaGreen => RGB(60, 179, 113);
    public static Color springGreen => RGB(0, 255, 127);
    public static Color mediumSpringGreen => RGB(0, 250, 154);
    public static Color lightSeaGreen => RGB(32, 178, 170);
    public static Color paleGreen => RGB(152, 251, 152);
    public static Color darkSeaGreen => RGB(143, 188, 143);

    // Cyans
    public static Color lightCyan => RGB(224, 255, 255);
    public static Color aquaMarine => RGB(127, 255, 212);
    public static Color turquoise => RGB(64, 224, 208);
    public static Color mediumTurquoise => RGB(72, 209, 204);
    public static Color darkTurquoise => RGB(0, 206, 209);

    // Blues
    public static Color lightBlue => RGB(173, 216, 230);
    public static Color skyBlue => RGB(135, 206, 235);
    public static Color lightSkyBlue => RGB(135, 206, 250);
    public static Color deepSkyBlue => RGB(0, 191, 255);
    public static Color dodgerBlue => RGB(30, 144, 255);
    public static Color cornflowerBlue => RGB(100, 149, 237);
    public static Color steelBlue => RGB(70, 130, 180);
    public static Color royalBlue => RGB(65, 105, 225);
    public static Color midnightBlue => RGB(25, 25, 112);
    public static Color navy => RGB(0, 0, 128);
    public static Color darkBlue => RGB(0, 0, 139);
    public static Color mediumBlue => RGB(0, 0, 205);

    // Purples
    public static Color purple => RGB(128, 0, 128);
    public static Color lavender => RGB(230, 230, 250);
    public static Color thistle => RGB(216, 191, 216);
    public static Color plum => RGB(221, 160, 221);
    public static Color violet => RGB(238, 130, 238);
    public static Color orchid => RGB(218, 112, 214);
    public static Color mediumOrchid => RGB(186, 85, 211);
    public static Color darkOrchid => RGB(153, 50, 204);
    public static Color blueViolet => RGB(138, 43, 226);
    public static Color darkViolet => RGB(148, 0, 211);

    // Browns
    public static Color cornSilk => RGB(255, 248, 220);
    public static Color blanchedAlmond => RGB(255, 235, 205);
    public static Color bisque => RGB(255, 228, 196);
    public static Color navajoWhite => RGB(255, 222, 173);
    public static Color wheat => RGB(245, 222, 179);
    public static Color burlyWood => RGB(222, 184, 135);
    public static Color tan => RGB(210, 180, 140);
    public static Color rosyBrown => RGB(188, 143, 143);
    public static Color sandyBrown => RGB(244, 164, 96);
    public static Color goldenRod => RGB(218, 165, 32);
    public static Color darkGoldenRod => RGB(184, 134, 11);
    public static Color peru => RGB(205, 133, 63);
    public static Color chocolate => RGB(210, 105, 30);
    public static Color saddleBrown => RGB(139, 69, 19);
    public static Color sienna => RGB(160, 82, 45);
    public static Color brown => RGB(165, 42, 42);

    // Whites
    public static Color snow => RGB(255, 250, 250);
    public static Color honeyDew => RGB(240, 255, 240);
    public static Color mintCream => RGB(245, 255, 250);
    public static Color azure => RGB(240, 255, 255);
    public static Color aliceBlue => RGB(240, 248, 255);
    public static Color ghostWhite => RGB(248, 248, 255);
    public static Color whiteSmoke => RGB(245, 245, 245);
    public static Color seashell => RGB(255, 245, 238);
    public static Color beige => RGB(245, 245, 220);
    public static Color oldLace => RGB(253, 245, 230);
    public static Color floralWhite => RGB(255, 250, 240);
    public static Color ivory => RGB(255, 255, 240);

    // Grays
    public static Color gainsboro => RGB(220, 220, 220);
    public static Color lightGray => RGB(211, 211, 211);
    public static Color silver => RGB(192, 192, 192);
    public static Color darkGray => RGB(169, 169, 169);
    public static Color dimGray => RGB(105, 105, 105);
    public static Color slateGray => RGB(112, 128, 144);
    public static Color darkSlateGray => RGB(47, 79, 79);
}
