﻿namespace OregonTrail2.PlayerInteraction
{
    public interface IPlayerInteraction
    {
        PlayerInput GetInput(params string[] prompt);
        Operation WriteMessage(params string[] prompt);
        Operation WriteMessageConditional(bool condition, params string[] promtp);
    }
}
