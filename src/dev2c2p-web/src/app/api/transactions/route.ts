import { NextRequest, NextResponse } from 'next/server';

const endpointUrl = process.env.API_ENDPOINT_URL;

export async function GET(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const apiEndpointUrl = `${endpointUrl}/transactions?${searchParams}`;

    const requestOptions: RequestInit = {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      cache: 'no-store',
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
