namespace ChessApp.Services.PieceNotationService.Entity;

public class Move
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int MoveNumber { get; set; }
    public string Color { get; set; } = string.Empty;
    
    /// <summary>
    /// Example: "K", "Q", "R", "B", "N" or "" (for pawn)
    /// </summary>
    public String NotationString { get; set; } = "";
    public string FromSquare { get; set; } = string.Empty;
    public string ToSquare { get; set; } = string.Empty;
    
    public bool IsCapture   { get; set; }
    public bool IsCheck     { get; set; }
    public bool IsCheckmate { get; set; }
    public bool IsKingSideCastle { get; set; }
    public bool IsQueenSideCastle { get; set; }
    public bool IsPawnPromotion { get; set; }
}