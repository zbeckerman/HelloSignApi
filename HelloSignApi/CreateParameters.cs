﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// this file contains objects for creating new things.

namespace HelloSignApi
{
    /// <summary>
    /// Object used to create a new unclaimed draft.
    /// </summary>
    public class NewUnclaimedDraft
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewUnclaimedDraft"/> class.
        /// </summary>
        public NewUnclaimedDraft()
        {
            Files = new PendingFileCollection();
            Signers = new List<Signer>();
            CcEmailAddresses = new List<string>();
            Metadata = new Dictionary<string, string>();
        }

        /// <summary>
        /// Whether this is a test, the signature request created from this draft will not be legally binding if <code>true</code>.
        /// </summary>
        public bool TestMode { get; set; }

        /// <summary>
        /// Gets the files to be uploaded.
        /// </summary>
        public PendingFileCollection Files { get; private set; }

        /// <summary>
        /// The type of unclaimed draft to create. Use values from <see cref="UnclaimedDraftTypes"/>.
        /// If the type is <see cref="UnclaimedDraftTypes.RequestSignature"/> then signers name and 
        /// email_address are not optional.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The subject in the email that will be sent to the signers.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The custom message in the email that will be sent to the signers.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The signers to request signatures.
        /// </summary>
        public IList<Signer> Signers { get; private set; }

        /// <summary>
        /// The email addresses that should be CCed.
        /// </summary>
        public IList<string> CcEmailAddresses { get; private set; }

        /// <summary>
        /// The URL you want signers redirected to after they successfully sign.
        /// </summary>
        public string SigningRedirectUrl { get; set; }

        /// <summary>
        /// Key-value data that should be attached to the signature request. 
        /// This metadata is included in all API responses and events involving the signature request. 
        /// For example, use the metadata field to store a signer's order number for look up when 
        /// receiving events for the signature request.
        /// Each request can include up to 10 metadata keys, with key names up to 40 characters long and 
        /// values up to 500 characters long.
        /// </summary>
        public IDictionary<string, string> Metadata { get; private set; }
    }

    /// <summary>
    /// Object used to create a new embedded unclaimed draft.
    /// </summary>
    public class NewEmbeddedUnclaimedDraft : NewUnclaimedDraft
    {
        /// <summary>
        /// Client id of the app you're using to create this draft. 
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The email address of the user that should be designated as the requester of this draft, 
        /// if the draft type is <see cref="UnclaimedDraftTypes.RequestSignature"/>.
        /// </summary>
        public string RequesterEmailAddress { get; set; }

        /// <summary>
        /// The request created from this draft will also be signable in embedded mode if set to <code>true</code>.
        /// </summary>
        public bool IsForEmbeddedSigning { get; set; }
    }

    ///// <summary>
    ///// Object used to create a new embedded unclaimed draft with template.
    ///// </summary>
    //public class NewEmbeddedUnclaimedDraftWithTemplate : NewEmbeddedUnclaimedDraft
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="NewEmbeddedUnclaimedDraftWithTemplate"/> class.
    //    /// </summary>
    //    public NewEmbeddedUnclaimedDraftWithTemplate()
    //    {
    //        TemplateIds = new List<string>();
    //    }

    //    /// <summary>
    //    /// The templates to create a request from. The order of the template is significant.
    //    /// </summary>
    //    public IList<string> TemplateIds { get; private set; }

    //    /// <summary>
    //    /// The title you want to assign to the SignatureRequest.
    //    /// </summary>
    //    public string Title { get; set; }

    //    /// <summary>
    //    /// The URL you want signers redirected to after they successfully request a signature.
    //    /// </summary>
    //    public string RequestingRedirectUrl { get; set; }
    //}

    /// <summary>
    /// Represents a signature request recipient.
    /// </summary>
    public class Signer
    {
        /// <summary>
        /// The name of the signer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email address of the signer. This is required.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The order the signer is required to sign.
        /// </summary>
        public int Order { get; set; }


    }

    /// <summary>
    /// Container object for files to be uploaded.
    /// </summary>
    public class PendingFileCollection : IList<PendingFile>
    {
        List<PendingFile> _list = new List<PendingFile>();

        private void VerifyTheSame(PendingFile item)
        {
            if (item.RemotePath == null)
            {
                if (_list.Any(f => f.RemotePath != null))
                {
                    throw new ArgumentException("File collection can only contain one type of file source.");
                }
            }
            else if (_list.Any(f => f.RemotePath == null))
            {
                throw new ArgumentException("File collection can only contain one type of file source.");
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PendingFile"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PendingFile"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public PendingFile this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count { get { return _list.Count; } }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(PendingFile item)
        {
            VerifyTheSame(item);
            _list.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(PendingFile item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(PendingFile[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<PendingFile> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(PendingFile item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, PendingFile item)
        {
            VerifyTheSame(item);
            _list.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(PendingFile item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }


    }

    /// <summary>
    /// Represents a file for creating signature requests.
    /// </summary>
    public class PendingFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingFile" /> class.
        /// </summary>
        /// <param name="remoteFilePath">The remote file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentException">Only remote http/https file is supported.</exception>
        public PendingFile(Uri remoteFilePath, string fileName = null)
        {
            if (string.Equals(remoteFilePath.Scheme, Uri.UriSchemeHttp) ||
                string.Equals(remoteFilePath.Scheme, Uri.UriSchemeHttps))
            {
                RemotePath = remoteFilePath;
                FileName = fileName ?? Path.GetFileName(remoteFilePath.AbsolutePath);
            }
            else
            {
                throw new ArgumentException("Only remote http/https file is supported.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingFile" /> class.
        /// </summary>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="fileName">Name of the file.</param>
        public PendingFile(string localFilePath, string fileName = null)
        {
            LocalPath = localFilePath;
            FileName = fileName ?? Path.GetFileName(localFilePath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingFile"/> class.
        /// </summary>
        /// <param name="fileData">The file data.</param>
        /// <param name="fileName">Name of the file.</param>
        public PendingFile(byte[] fileData, string fileName)
        {
            Stream = new MemoryStream(fileData);
            FileName = fileName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingFile"/> class.
        /// </summary>
        /// <param name="fileData">The file data.</param>
        /// <param name="fileName">Name of the file.</param>
        public PendingFile(Stream fileData, string fileName)
        {
            Stream = fileData;
            FileName = fileName;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the remote file path.
        /// </summary>
        public Uri RemotePath { get; private set; }

        /// <summary>
        /// Gets the local file path.
        /// </summary>
        public string LocalPath { get; private set; }

        /// <summary>
        /// Gets the stream data.
        /// </summary>
        public Stream Stream { get; private set; }

    }
}
