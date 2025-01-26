//source: https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm
float smin(float a, float b, float k)
{
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * (1.0 / 4.0);
}

//source: https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm
float sdCircle(float2 p, float r)
{
    return length(p) - r;
}