public static class WordGenerator
{
    private static string[] easyWords = {
        "gato", "perro", "sol", "rojo", "mar", "pan", "luz"
    };

    public static string GetRandomWord()
    {
        return easyWords[UnityEngine.Random.Range(0, easyWords.Length)];
    }
}
