import { NextRequest, NextResponse } from 'next/server';

const endpointUrl = process.env.API_ENDPOINT_URL;

export async function POST(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const apiEndpointUrl = `${endpointUrl}/upload?${searchParams}`;

    const requestOptions: RequestInit = {
      method: 'POST',
      body: await request.formData(),
    };

    const response = await fetch(apiEndpointUrl, requestOptions);
    const data = await response.json();

    if (response.ok) {
      return NextResponse.json(data);
    }

    return NextResponse.json(data, { status: response.status });
  } catch (error) {
    return NextResponse.json(error, { status: 500 });
  }
}
