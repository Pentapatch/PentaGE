using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Provides extension methods for converting <see cref="Matrix4x4"/> and <see cref="Matrix3x2"/> objects to float arrays.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Converts a <see cref="Matrix4x4"/> object to a float array representation.
        /// </summary>
        /// <param name="source">The <see cref="Matrix4x4"/> object to convert.</param>
        /// <returns>A float array containing the elements of the matrix in row-major order.</returns>
        public static float[] ToArray(this Matrix4x4 source)
        {
            float[] matrixArray = new float[16];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixArray[i * 4 + j] = source[i, j];
                }
            }

            return matrixArray;
        }

        /// <summary>
        /// Converts a <see cref="Matrix3x2"/> object to a float array representation.
        /// </summary>
        /// <param name="source">The <see cref="Matrix3x2"/> object to convert.</param>
        /// <returns>A float array containing the elements of the matrix in row-major order.</returns>
        public static float[] ToArray(this Matrix3x2 source)
        {
            float[] matrixArray = new float[6];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    matrixArray[i * 3 + j] = source[i, j];
                }
            }

            return matrixArray;
        }

    }
}