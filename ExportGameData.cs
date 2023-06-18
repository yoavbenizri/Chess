
[System.Serializable]
public class ExportGameData 
{
    public bool WhiteTurn;
    public bool WhiteCanCastleLeft;
    public bool WhiteCanCastleRight;
    public bool BlackCanCastleLeft;
    public bool BlackCanCastleRight;
    public string FENPieces;
    public string nameW;
    public string nameB;
    public bool easyMode;
    public bool nameEntered = false;

    public ExportGameData(bool WhiteTurn, bool WhiteCanCastleLeft, bool WhiteCanCastleRight, bool BlackCanCastleLeft,
        bool BlackCanCastleRight, string FENPieces, string nameW, string nameB, bool easyMode, bool nameEntered)
    {
        this.WhiteTurn = WhiteTurn;
        this.WhiteCanCastleLeft = WhiteCanCastleLeft;
        this.WhiteCanCastleRight = WhiteCanCastleRight;
        this.BlackCanCastleLeft = BlackCanCastleLeft;
        this.BlackCanCastleRight = BlackCanCastleRight;
        this.FENPieces = FENPieces;
        this.nameW = nameW;
        this.nameB = nameB;
        this.easyMode = easyMode;
        this.nameEntered = nameEntered;
    }

    public static ExportGameData ExportDataFromController(GameController controller)
    {
        string fen = controller.GetFEN(controller.Pieces);
        return new ExportGameData(controller.WhiteTurn, controller.WhiteCanCastleLeft, controller.WhiteCanCastleRight,
            controller.BlackCanCastleLeft, controller.BlackCanCastleRight,
            fen, controller.nameW, controller.nameB, controller.easyMode, controller.nameEntered);
    }
    
    
    public void ImportToController(GameController controller)
    {
        string[,] pieces = controller.FENToPieces(this.FENPieces);
        controller.WhiteTurn = WhiteTurn;
        controller.WhiteCanCastleLeft = WhiteCanCastleLeft;
        controller.WhiteCanCastleRight = WhiteCanCastleRight;
        controller.BlackCanCastleLeft = BlackCanCastleLeft;
        controller.BlackCanCastleRight = BlackCanCastleRight;
        controller.Pieces = pieces;
        controller.nameW = nameW;
        controller.nameB = nameB;
        controller.easyMode = easyMode;
        controller.nameEntered = nameEntered;
    }
}
