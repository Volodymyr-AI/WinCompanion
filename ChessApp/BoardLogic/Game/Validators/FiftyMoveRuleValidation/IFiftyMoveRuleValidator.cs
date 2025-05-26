using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.BoardLogic.Game.Validators.FiftyMoveRuleValidation;

public interface IFiftyMoveRuleValidator
{
    void UpdateAfterMove(Move move);
    bool IsFiftyMoveRuleDraw();
}