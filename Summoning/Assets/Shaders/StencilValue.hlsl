#ifndef STENCILVALUE_INCLUDED
#define STENCILVALUE_INCLUDED

void StencilVal_int(float2 UV, out int Out) {
	float2 sobel = 0;

	[unroll] for (int i = 0; i < 9; i++) {
		float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
		sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
	}

	Out = length(sobel);
}

#endif