using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoardSlot : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public MatchObject matchObjectInSlot;
    public Image matchObjectImage;
    public DraggedTile draggingTile;

    public int row;
    public int col;

    public Board boardAttachedTo;
    public void updateMatchObjectInSlot(Sprite newSprite, matchType newMatchType)
    {
        matchObjectInSlot.matchIcon = newSprite;
        matchObjectImage.sprite = matchObjectInSlot.matchIcon;
        matchObjectInSlot.tileMatchType = newMatchType;
    }

    public void swapBoardSlots(BoardSlot boardSlotToSwapWith)
    {
        Sprite tempSprite = matchObjectInSlot.matchIcon;
        matchType tempTileType = matchObjectInSlot.tileMatchType;

        matchObjectInSlot.matchIcon = boardSlotToSwapWith.matchObjectInSlot.matchIcon;
        matchObjectInSlot.tileMatchType = boardSlotToSwapWith.matchObjectInSlot.tileMatchType;

        matchObjectImage.sprite = matchObjectInSlot.matchIcon;

        boardSlotToSwapWith.matchObjectInSlot.matchIcon = tempSprite;
        boardSlotToSwapWith.matchObjectInSlot.tileMatchType = tempTileType;

        boardSlotToSwapWith.matchObjectImage.sprite = boardSlotToSwapWith.matchObjectInSlot.matchIcon;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (draggingTile.slotDraggedFrom != null)
        {
            if (draggingTile.hoveredSlot != null)
            {
                // Left Right Dragging
                if (row == draggingTile.hoveredSlot.row)
                {
                    if(!draggingTile.dragLeftRight)
                    {
                        int rowDifference = draggingTile.slotDraggedFrom.row - draggingTile.hoveredSlot.row;
                        int colDifference = draggingTile.slotDraggedFrom.col - draggingTile.hoveredSlot.col;

                        // Positive difference, means hovered slot is higher, on board and we must move the tiles back down
                        if (rowDifference > 0)
                        {

                            for (int i = rowDifference; i > 0; i--)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row - i, draggingTile.slotDraggedFrom.col].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row - i + 1, draggingTile.slotDraggedFrom.col]);
                            }
                        }
                        // Negative difference, means hovered slot is lower, on board and we must move tiles back up.
                        else
                        {
                            for (int i = Mathf.Abs(rowDifference); i > 0; i--)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row + i, draggingTile.slotDraggedFrom.col].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row + i - 1, draggingTile.slotDraggedFrom.col]);
                            }
                        }

                        if (colDifference > 0)
                        {
                            for (int i = 0; i < colDifference; i++)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col - i].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col - i - 1]);
                            }
                        }
                        // Negative difference, means hovered slot is lower, on board and we must move tiles back up.
                        else
                        {
                            for (int i = 0; i < Mathf.Abs(colDifference); i++)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col + i].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col + i + 1]);
                            }
                        }
                        draggingTile.dragLeftRight = true;
                    }


                    // Move Right
                    if (col > draggingTile.hoveredSlot.col)
                    {
                        boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, col].swapBoardSlots(boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, col - 1]);
                    }
                    // Move Left
                    else if (col < draggingTile.hoveredSlot.col)
                    {
                        boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, col].swapBoardSlots(boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, col + 1]);
                    }

                }

                // Up Down Dragging
                else if (col == draggingTile.hoveredSlot.col)
                {
                    if (draggingTile.dragLeftRight)
                    {
                        int colDifference = draggingTile.slotDraggedFrom.col - draggingTile.hoveredSlot.col;
                        int rowDifference = draggingTile.slotDraggedFrom.row - draggingTile.hoveredSlot.row;

                        // Positive difference, means hovered slot is higher, on board and we must move the tiles back down
                        if (colDifference > 0)
                        {

                            for (int i = colDifference; i > 0; i--)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col - i].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col - i + 1]);
                            }
                        }
                        // Negative difference, means hovered slot is lower, on board and we must move tiles back up.
                        else
                        {
                            for (int i = Mathf.Abs(colDifference); i > 0; i--)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col + i].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row, draggingTile.slotDraggedFrom.col + i - 1]);
                            }
                        }

                        if (rowDifference > 0)
                        {
                            for (int i = 0; i < rowDifference; i++)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row - i, draggingTile.slotDraggedFrom.col].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row - i - 1, draggingTile.slotDraggedFrom.col]);
                            }
                        }
                        // Negative difference, means hovered slot is lower, on board and we must move tiles back up.
                        else
                        {
                            for (int i = 0; i < Mathf.Abs(rowDifference); i++)
                            {
                                boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row + i, draggingTile.slotDraggedFrom.col].swapBoardSlots
                                    (boardAttachedTo.boardSlots[draggingTile.slotDraggedFrom.row + i + 1, draggingTile.slotDraggedFrom.col]);
                            }
                        }
                        draggingTile.dragLeftRight = false;
                    }

                    // Move Down
                    if (row > draggingTile.hoveredSlot.row)
                    {
                        boardAttachedTo.boardSlots[row, draggingTile.slotDraggedFrom.col].swapBoardSlots(boardAttachedTo.boardSlots[row - 1, draggingTile.slotDraggedFrom.col]);
                    }
                    // Move Up
                    else if (row < draggingTile.hoveredSlot.row)
                    {
                        boardAttachedTo.boardSlots[row, draggingTile.slotDraggedFrom.col].swapBoardSlots(boardAttachedTo.boardSlots[row + 1, draggingTile.slotDraggedFrom.col]);
                    }

                }

                draggingTile.hoveredSlot = this;
            }

        }
        draggingTile.hoveredSlot = this;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {

    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(!(matchObjectInSlot.tileMatchType == matchType.block))
            draggingTile.slotDraggedFrom = this;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (draggingTile.slotDraggedFrom != null)
        {
            if (draggingTile.dragLeftRight)
            {
                boardAttachedTo.checkBoardMatch(draggingTile.slotDraggedFrom.row, draggingTile.hoveredSlot.col);
            }
            else
            {
                boardAttachedTo.checkBoardMatch(draggingTile.hoveredSlot.row, draggingTile.slotDraggedFrom.col);
            }
        }
        draggingTile.slotDraggedFrom = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggingTile.slotDraggedFrom != null)
        {
            draggingTile.updateDraggedObject(matchObjectInSlot);
            draggingTile.draggedObject.matchIcon = matchObjectInSlot.matchIcon;
            draggingTile.draggedObject.tileMatchType = matchObjectInSlot.tileMatchType;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //draggingTile.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

}
