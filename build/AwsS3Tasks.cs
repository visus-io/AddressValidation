// ReSharper disable InconsistentNaming

using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

internal sealed class AwsS3Tasks(AmazonS3Client client)
{
	private readonly AmazonS3Client _client = client ?? throw new ArgumentNullException(nameof(client));

	public ValueTask EmptyAsync(string bucketName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(bucketName);
		return EmptyInternalAsync(bucketName);
	}

	public ValueTask UploadAsync(string directory, string bucketName)
	{
		ArgumentException.ThrowIfNullOrEmpty(directory);
		ArgumentException.ThrowIfNullOrEmpty(bucketName);

		return UploadInternalAsync(directory, bucketName);
	}

	private async ValueTask EmptyInternalAsync(string bucketName)
	{
		ListObjectsResponse items = await _client.ListObjectsAsync(bucketName)
												 .ConfigureAwait(false);

		do
		{
			if ( items.S3Objects is null )
			{
				break;
			}

			foreach ( S3Object obj in items.S3Objects )
			{
				await client.DeleteObjectAsync(obj.BucketName, obj.Key).ConfigureAwait(false);
			}
		} while ( items.IsTruncated ?? false );
	}

	private async ValueTask UploadInternalAsync(string directory, string bucketName)
	{
		TransferUtility transfer = new(_client);

		await transfer.UploadDirectoryAsync(directory, bucketName, "*.*", SearchOption.AllDirectories)
					  .ConfigureAwait(false);
	}
}
