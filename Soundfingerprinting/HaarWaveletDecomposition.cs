﻿namespace Soundfingerprinting.Fingerprinting.Wavelets
{
	using System;

	public abstract class HaarWaveletDecomposition : IWaveletDecomposition
	{
		public abstract void DecomposeImageInPlace(double[][] image);

		protected void DecompositionStep(double[] array, int h)
		{
			double[] temp = new double[h];

			h /= 2;
			for (int i = 0; i < h; i++)
			{
				temp[i] = (double)((array[2 * i] + array[(2 * i) + 1]) / Math.Sqrt(2.0));
				temp[i + h] = (double)((array[2 * i] - array[(2 * i) + 1]) / Math.Sqrt(2.0));
			}

			for (int i = 0; i < (h * 2); i++)
			{
				array[i] = temp[i];
			}
		}
	}
}