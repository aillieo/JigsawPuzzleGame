// -----------------------------------------------------------------------
// <copyright file="Board.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections;
    using System.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(DragEvents))]
    [RequireComponent(typeof(GameUI))]
    public class Board : MonoBehaviour
    {
        public Texture2D image;
        public Vector2Int dimension;

        private Piece[] managedPieces;
        private Piece draggingPiece;

        private float positionFixThreshold = 0.2f;

        [ContextMenu("Generate")]
        private IEnumerator Generate()
        {
            var size = new Vector2Int(this.image.width, this.image.height);
            var pieceCount = this.dimension.x * this.dimension.y;
            this.managedPieces = new Piece[pieceCount];
            var context = new CuttingContext(size, this.dimension);
            for (var i = 0; i < pieceCount; i++)
            {
                var pieceData = PieceCreator.CalculatePieceData(context, i);
                var pieceObject = Piece.Create(this.image, pieceData);
                this.managedPieces[i] = pieceObject;
                pieceObject.transform.localPosition = this.GetExpectedPosition(pieceData);
                yield return new WaitForSeconds(0.1f);
            }
        }

        [ContextMenu("Shuffle")]
        private void Shuffle()
        {
            var pieceCount = this.dimension.x * this.dimension.y;
            for (var i = 0; i < pieceCount; i++)
            {
                var piece = this.managedPieces[i];
                var position = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                this.StartCoroutine(TweetUtils.Move(piece.gameObject, position, 1f));
            }
        }

        private void RestartGame()
        {
            this.StartCoroutine(this.CoRestartGame());
        }

        private IEnumerator CoRestartGame()
        {
            if (this.managedPieces != null)
            {
                foreach (var piece in this.managedPieces)
                {
                    Destroy(piece.gameObject);
                }

                this.managedPieces = null;
            }

            yield return this.Generate();
            this.Shuffle();

            var gameUI = this.GetComponent<GameUI>();
            gameUI.isPlaying = true;
            gameUI.gameStartTime = Time.time;
        }

        private void Start()
        {
            this.RestartGame();
        }

        private void OnEnable()
        {
            var dragEvents = this.GetComponent<DragEvents>();
            dragEvents.OnDragBegin += this.OnDragBegin;
            dragEvents.OnDrag += this.OnDrag;
            dragEvents.OnDragEnd += this.OnDragEnd;

            var gameUI = this.GetComponent<GameUI>();
            gameUI.onRestartClicked += this.RestartGame;
        }

        private void OnDisable()
        {
            var dragEvents = this.GetComponent<DragEvents>();
            dragEvents.OnDragBegin -= this.OnDragBegin;
            dragEvents.OnDrag -= this.OnDrag;
            dragEvents.OnDragEnd -= this.OnDragEnd;

            var gameUI = this.GetComponent<GameUI>();
            gameUI.onRestartClicked -= this.RestartGame;
        }

        private void OnDragBegin(Vector2 position)
        {
            var piece = this.FindPiece(position);
            if (piece != null)
            {
                this.draggingPiece = piece;
                this.draggingPiece.OnDragBegin(position);
                this.UpdatePiecePosition(this.draggingPiece, position);
            }
        }

        private void OnDrag(Vector2 position)
        {
            if (this.draggingPiece != null)
            {
                this.UpdatePiecePosition(this.draggingPiece, position);
                this.draggingPiece.OnDrag(position);
            }
        }

        private void OnDragEnd(Vector2 position)
        {
            if (this.draggingPiece != null)
            {
                this.draggingPiece.OnDragEnd(position);
                var fixHappen = this.FixPiecePosition(this.draggingPiece, position);
                this.draggingPiece = null;

                if (fixHappen)
                {
                    this.CheckGameEnd();
                }
            }
        }

        private Piece FindPiece(Vector2 screenPosition)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(screenPosition);

            var hitColliders = new Collider2D[8];
            var hitCount = Physics2D.OverlapPointNonAlloc(worldPoint, hitColliders, -1, -100, 100);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider != null)
                {
                    var piece = hitCollider.GetComponent<Piece>();
                    if (piece != null)
                    {
                        return piece;
                    }
                }
            }

            return null;
        }

        private Vector2 GetExpectedPosition(PieceData pieceData)
        {
            var start = -new Vector2(this.image.width, this.image.height) * 0.5f;
            var centerPosition = start + pieceData.rawRect.center;
            return centerPosition / 100f;
        }

        private void UpdatePiecePosition(Piece piece, Vector2 screenPosition)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            var position = piece.transform.position;
            position.x = worldPosition.x;
            position.y = worldPosition.y;
            position.z = this.transform.position.z - 1f;
            piece.transform.position = position;
        }

        private bool FixPiecePosition(Piece piece, Vector2 screenPosition)
        {
            var position = piece.transform.position;

            var pieceData = piece.pieceData;
            var expectedPosition = this.GetExpectedPosition(pieceData);
            if (Vector2.Distance(position, expectedPosition) < this.positionFixThreshold)
            {
                position = expectedPosition;
                position.z = this.transform.position.z;
                piece.transform.position = position;
                return true;
            }
            else
            {
                position.z = this.transform.position.z;
                piece.transform.position = position;
                return false;
            }
        }

        private void CheckGameEnd()
        {
            var pieceCount = this.dimension.x * this.dimension.y;
            for (var i = 0; i < pieceCount; i++)
            {
                var piece = this.managedPieces[i];
                var pieceData = piece.pieceData;
                var expectedPosition = this.GetExpectedPosition(pieceData);
                if (Vector2.Distance(piece.transform.position, expectedPosition) > this.positionFixThreshold)
                {
                    return;
                }
            }

            var gameUI = this.GetComponent<GameUI>();
            gameUI.isPlaying = false;
            gameUI.gemeEndTime = Time.time;
        }
    }
}
