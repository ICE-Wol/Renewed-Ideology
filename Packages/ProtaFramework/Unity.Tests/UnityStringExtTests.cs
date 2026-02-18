using NUnit.Framework;
using Prota.Unity;

public class UnityStringExtTests
{
    #region ParseSpritePath - 带 extensionName 参数的重载
    
    [Test]
    public void ParseSpritePath_NullPath_ReturnsFalse()
    {
        // Arrange
        string path = null;
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(texturePath);
        Assert.IsNull(spriteName);
    }
    
    [Test]
    public void ParseSpritePath_EmptyPath_ReturnsFalse()
    {
        // Arrange
        string path = "";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(texturePath);
        Assert.IsNull(spriteName);
    }
    
    [Test]
    public void ParseSpritePath_SimplePath_WithoutBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        // 没有括号时，firstIndexOfBackBracket = -1，path[.. -1] 会取到倒数第二个字符之前（不包括最后一个字符）
        Assert.AreEqual("Path/To/Texture.png", texturePath);
        Assert.AreEqual("Texture", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_PathWithSingleBrackets_ExtractsCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture]SpriteName";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_MultipleOpenBrackets_ReturnsFalse()
    {
        // Arrange
        string path = "Path/To/Texture[First[Second]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_MultipleCloseBrackets_ReturnsFalse()
    {
        // Arrange
        string path = "Path/To/Texture[First]Second]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_MultipleBrackets_ReturnsFalse()
    {
        // Arrange
        string path = "Path/To/Texture[First][Second]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_OpenBracketWithoutClose_ReturnsFalse()
    {
        // Arrange
        string path = "Path/To/Texture[SpriteName";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_CloseBracketWithoutOpen_ReturnsFalse()
    {
        // Arrange
        string path = "Path/To/Texture]SpriteName]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_EmptyBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture[]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture", texturePath);
        Assert.AreEqual("", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_ExtensionNameWithoutDot_AddsDot()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath("png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture.png", texturePath);
        Assert.AreEqual("Texture", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_ExtensionNameWithDot_KeepsAsIs()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture.png", texturePath);
        Assert.AreEqual("Texture", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_ExtensionNameWithDot_WithBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture[SpriteName]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture", texturePath);
        Assert.AreEqual("SpriteName", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_ExtensionNameWithoutDot_WithBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture[SpriteName]";
        
        // Act
        bool result = path.ParseSpritePath("png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture", texturePath);
        Assert.AreEqual("SpriteName", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_ExtensionNameOnlyDot_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath(".", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture.png", texturePath);
        Assert.AreEqual("Texture", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_ExtensionNameOnlyDot_WithBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture[SpriteName]";
        
        // Act
        bool result = path.ParseSpritePath(".", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture", texturePath);
        Assert.AreEqual("SpriteName", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_NullExtensionName_TreatsAsEmpty()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath(null, out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
    }
    
    [Test]
    public void ParseSpritePath_EmptyExtensionName_TreatsAsEmpty()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath("", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
    }
    
    [Test]
    public void ParseSpritePath_RootPathWithBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Texture[SpriteName]";
        
        // Act
        bool result = path.ParseSpritePath(".png", out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Texture", texturePath);
        Assert.AreEqual("SpriteName", spriteName);
    }
    
    #endregion
    
    #region ParseSpritePath - 不带 extensionName 参数的重载（默认使用 .png）
    
    [Test]
    public void ParseSpritePath_DefaultExtension_SimplePath_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture.png";
        
        // Act
        bool result = path.ParseSpritePath(out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture.png", texturePath);
        Assert.AreEqual("Texture", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_DefaultExtension_WithBrackets_WorksCorrectly()
    {
        // Arrange
        string path = "Path/To/Texture[SpriteName]";
        
        // Act
        bool result = path.ParseSpritePath(out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Path/To/Texture", texturePath);
        Assert.AreEqual("SpriteName", spriteName);
    }
    
    [Test]
    public void ParseSpritePath_DefaultExtension_MultipleBrackets_ReturnsFalse()
    {
        // Arrange
        string path = "Path/To/Texture[First][Second]";
        
        // Act
        bool result = path.ParseSpritePath(out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void ParseSpritePath_DefaultExtension_NullPath_ReturnsFalse()
    {
        // Arrange
        string path = null;
        
        // Act
        bool result = path.ParseSpritePath(out string texturePath, out string spriteName);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(texturePath);
        Assert.IsNull(spriteName);
    }
    
    #endregion
}
