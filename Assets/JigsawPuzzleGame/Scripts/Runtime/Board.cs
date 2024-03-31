// -----------------------------------------------------------------------
// <copyright file="Board.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
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

        [ContextMenu("Generate")]
        private async Task Generate()
        {
            var size = new Vector2Int(this.image.width, this.image.height);
            var pieceCount = this.dimension.x * this.dimension.y;
            this.managedPieces = new Piece[pieceCount];
            var context = new CuttingContext(size, this.dimension);
            for (var i = 0; i < pieceCount; i++)
            {
                var pieceData = await PieceCreator.CalculatePieceData(context, i);
                var pieceObject = Piece.Create(this.image, pieceData);
                this.managedPieces[i] = pieceObject;
                pieceObject.transform.localPosition = (Vector3)(Vector2)pieceData.extendedRect.position / 100f;
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

        private async void RestartGame()
        {
            if (this.managedPieces != null)
            {
                foreach (var piece in this.managedPieces)
                {
                    Destroy(piece.gameObject);
                }

                this.managedPieces = null;
            }

            await this.Generate();
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
                this.FixPiecePosition(this.draggingPiece, position);
                this.draggingPiece = null;

                this.CheckGameEnd();
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

        private void UpdatePiecePosition(Piece piece, Vector2 screenPosition)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            var position = piece.transform.position;
            position.x = worldPosition.x;
            position.y = worldPosition.y;
            position.z = this.transform.position.z - 1f;
            piece.transform.position = position;
        }

        private void FixPiecePosition(Piece piece, Vector2 screenPosition)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            var position = piece.transform.position;

            var pieceData = piece.pieceData;
            var expectedPosition = (Vector3)(Vector2)pieceData.extendedRect.position / 100f;
            if (Vector2.Distance(position, expectedPosition) < 1f)
            {
                position = expectedPosition;
            }

            position.z = this.transform.position.z;
            piece.transform.position = position;
        }

        private void CheckGameEnd()
        {
            var pieceCount = this.dimension.x * this.dimension.y;
            for (var i = 0; i < pieceCount; i++)
            {
                var piece = this.managedPieces[i];
                var pieceData = piece.pieceData;
                var expectedPosition = (Vector3)(Vector2)pieceData.extendedRect.position / 100f;
                if (Vector2.Distance(piece.transform.position, expectedPosition) > 1f)
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
