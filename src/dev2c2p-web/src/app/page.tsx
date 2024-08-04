'use client';

import {
  Button,
  Card,
  CardBody,
  getKeyValue,
  Input,
  Select,
  SelectItem,
  Table,
  TableBody,
  TableCell,
  TableColumn,
  TableHeader,
  TableRow,
} from '@nextui-org/react';
import { useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { useCallback, useMemo, useRef, useState } from 'react';

export default function RootPage() {
  const [uploadDebugData, setUploadDebugData] = useState({});
  const [listDebugData, setListDebugData] = useState({});

  const inputFileRef = useRef<HTMLInputElement>(null);

  const handleSelectFilePress = useCallback(() => {
    inputFileRef.current?.click();
  }, []);

  const handleInputChange = useCallback(async () => {
    const file = inputFileRef.current?.files?.[0];

    if (!file) {
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch('/api/upload', {
        method: 'POST',
        body: formData,
      });
      const data = await response.json();

      if (response.ok) {
        setUploadDebugData({
          message: 'file uploaded successfully',
          data: data,
        });
      } else {
        setUploadDebugData({ message: 'file upload failed', data: data });
      }
    } catch (error) {
      setUploadDebugData({ message: 'file upload failed', data: error });
    }
  }, []);

  const [inputs, setInputs] = useState({
    currency: '',
    status: '',
    from: '',
    to: '',
  });

  const dataQuery = useQuery<{ id: string; payment: string; status: string }[]>(
    {
      queryKey: ['transactions', inputs],
      queryFn: async ({ signal }) => {
        try {
          const response = await axios.request({
            url: `/api/transactions`,
            method: 'GET',
            headers: {},
            params: {
              filter: inputs,
            },
            signal,
          });
          setListDebugData({
            message: 'data fetched successfully',
            data: response.data.data,
          });

          return response.data.data;
        } catch (error) {
          setListDebugData({
            message: 'failed to fetch data',
            data: (error as { response: { data: { errors: string[] } } })
              .response.data.errors,
          });
        }
      },
      enabled: true,
    },
  );

  const data = useMemo(() => {
    return dataQuery.data ?? [];
  }, [dataQuery.data]);

  const columns = useMemo(
    () => [
      {
        key: 'id',
        label: 'id',
      },
      {
        key: 'payment',
        label: 'payment',
      },
      {
        key: 'status',
        label: 'status',
      },
    ],
    [],
  );

  return (
    <div className="w-full h-full flex flex-col grow bg-background p-4 items-center justify-center gap-4">
      <div className="">
        <Button
          color="primary"
          className="bg-primary text-primary-foreground"
          onPress={handleSelectFilePress}
        >
          Select File
        </Button>
        <input
          ref={inputFileRef}
          type="file"
          className="hidden"
          onChange={handleInputChange}
        ></input>
      </div>
      <div className="px-4 min-w-32 max-w-full">
        Response: /upload
        <Card>
          <CardBody>
            <pre>{JSON.stringify(uploadDebugData, null, 2)}</pre>
          </CardBody>
        </Card>
      </div>
      <div className="flex flex-row gap-4">
        <Select
          className="min-w-48"
          placeholder="Select currency"
          label="Currency"
          items={[
            {
              key: 'THB',
              label: 'THB',
            },
            {
              key: 'USD',
              label: 'USD',
            },
            {
              key: 'EUR',
              label: 'EUR',
            },
          ]}
          value={inputs.currency}
          onChange={(value) => {
            setInputs((prev) => ({ ...prev, currency: value.target.value }));
          }}
        >
          {(item) => <SelectItem key={item.key}>{item.label}</SelectItem>}
        </Select>
        <Input
          label="From"
          placeholder="yyyy-MM-dd HH:mm:ss"
          value={inputs.from}
          onValueChange={(e) => setInputs((prev) => ({ ...prev, from: e }))}
        />
        <Input
          label="To"
          placeholder="yyyy-MM-dd HH:mm:ss"
          value={inputs.to}
          onValueChange={(e) => setInputs((prev) => ({ ...prev, from: e }))}
        />
        <Select
          className="min-w-48"
          placeholder="Select status"
          label="Status"
          items={[
            {
              key: 'A',
              label: 'Approved',
            },
            {
              key: 'R',
              label: 'Rejected/Failed',
            },
            {
              key: 'D',
              label: 'Done/Finished',
            },
          ]}
          value={inputs.status}
          onChange={(value) => {
            setInputs((prev) => ({ ...prev, status: value.target.value }));
          }}
        >
          {(item) => <SelectItem key={item.key}>{item.label}</SelectItem>}
        </Select>
      </div>
      <div className="flex flex-col grow">
        <div className="flex flex-col grow p-4 rounded-large shadow-small">
          <div className="grow h-full border-b-1 mb-4">
            <Table aria-label="Example table with dynamic content">
              <TableHeader columns={columns}>
                {(column) => (
                  <TableColumn key={column.key}>{column.label}</TableColumn>
                )}
              </TableHeader>
              <TableBody items={data}>
                {(item) => (
                  <TableRow key={item.id}>
                    {(columnKey) => (
                      <TableCell>{getKeyValue(item, columnKey)}</TableCell>
                    )}
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>
        </div>
        <div className="px-4 min-w-32 max-w-full">
          Response: /transactions
          <Card>
            <CardBody>
              <pre>{JSON.stringify(listDebugData, null, 2)}</pre>
            </CardBody>
          </Card>
        </div>
      </div>
    </div>
  );
}
