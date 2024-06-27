/**
 * @jest-environment jsdom
 */

'use strict';

const base64function = require("../../SFA.DAS.ApprenticeApp.Pwa/wwwroot/js/arrayBufferToBase64");

test("should return base64 string", () => {
    let buffer = new Uint8Array([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
    expect(base64function.arrayBufferToBase64(buffer)).toBe("AQIDBAUGBwgJCg==");
});