using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.BoardLogic.Game.Validators.FiftyMoveRuleValidation;

/// <summary>
/// Validator for 50 move rule
/// * Rule: if in the last 50 moves players did not move a single pawn - Stalemate
/// ** for counter used half moves ( 1 full move is a White and Black moves ) 
/// </summary>
public class FiftyMoveRuleValidator : IFiftyMoveRuleValidator
{
    private int _halfMoveCounter = 0; // counter of half moves without a pawn move or capture

    /// <summary>
    /// Update _moveCounter based on a last mover
    /// </summary>
    /// <param name="move"></param>
    public void UpdateAfterMove(Move move)
    {
        // If move is a pawn move or a capture - _moverCounter = 0
        if (string.IsNullOrEmpty(move.NotationString) || move.IsCapture)
        {
            _halfMoveCounter = 0;
        }
        else
        {
            // Increase counter
            _halfMoveCounter++;
        }
    }
    
    /// <summary>
    /// Checking, if counter exceedeed
    /// </summary>
    /// <returns></returns>
    public bool IsFiftyMoveRuleDraw()
    {
        return _halfMoveCounter >= 99; // 50 full moves = 100 half moves
    }
    
    /// <summary>
    /// Check current state of a counter  
    /// </summary>
    public int HalfMoveCounter=> _halfMoveCounter;

    /// <summary>
    /// Reset a counter ( if game is restarted )
    /// </summary>
    public void Reset()
    {
        _halfMoveCounter = 0;
    }

    /// <summary>
    /// Set counter ( for games that a being downloaded )
    /// </summary>
    /// <param name="counter"></param>
    public void SetHalfMoveCounter(int counter)
    {
        _halfMoveCounter = counter;
    }
}