#if !defined(FLOW_INCLUDED)
#define FLOW_INCLUDED

float2 FlowUV (float2 uv, float2 flowVector, float time) {
	float progress = frac(time);
	return uv - flowVector * progress;
}

#endif