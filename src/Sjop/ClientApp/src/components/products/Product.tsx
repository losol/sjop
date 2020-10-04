import React from "react";

function Product(props: any) {
  return (
    <>
      <div className="rounded overflow-hidden shadow-md m-3">
        <div className="px-6 py-4">
          <div className="font-bold text-xl mb-2">{props.name}</div>
          <p className="text-gray-700 text-base">{props.description}</p>
          <p>{props.price}</p>
        </div>
      </div>
    </>
  );
}

export default Product;
